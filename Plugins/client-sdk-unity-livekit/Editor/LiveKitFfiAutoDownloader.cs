// Copyright (c) Convai
// Auto-downloads LiveKit FFI binaries when the plugin is imported or when opened in the Editor,
// to avoid committing >100MB binaries to the repository.

#if UNITY_EDITOR
using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Convai.EditorTools
{
	[InitializeOnLoad]
	public static class LiveKitFfiAutoDownloader
	{
		// Source of prebuilt FFIs. Keep in sync with upstream.
		// Example: https://github.com/livekit/rust-sdks/releases/download/rust-sdks/livekit-ffi@0.12.26/ffi-macos-arm64.zip
		private const string RELEASES_BASE_URL = "https://github.com/livekit/rust-sdks/releases/download";
		private const string VERSION_TAG = "rust-sdks/livekit-ffi@0.12.33";

		// Paths relative to project root
		private const string PLUGIN_ROOT_RELATIVE = "Assets/Convai/Plugins/client-sdk-unity-livekit";
		private static readonly string _projectRoot = Path.GetDirectoryName(Application.dataPath);
		private static readonly string _pluginRoot = Path.Combine(_projectRoot ?? string.Empty, PLUGIN_ROOT_RELATIVE);
		private static readonly string _downloadsDir = Path.Combine(_pluginRoot, "downloads~");
		private static readonly string _pluginsDir = Path.Combine(_pluginRoot, "Runtime/Plugins");
		private static readonly HttpClient _httpClient = new();

		static LiveKitFfiAutoDownloader()
		{
			// Run once per editor session
			EditorApplication.delayCall += async () =>
			{
				try
				{
					if (await EnsureEditorFfiInstalledAsync())
					{
						AssetDatabase.Refresh();
					}
				}
				catch (Exception ex)
				{
					Debug.LogWarning($"LiveKit FFI auto-download failed: {ex.Message}\n{ex}");
				}
			};
		}

		[MenuItem("Convai/LiveKit/Download FFI for current platform", priority = 1000)]
		private static async void MenuDownloadEditorFfi()
		{
			try
			{
				if (await EnsureEditorFfiInstalledAsync(force: true))
				{
					AssetDatabase.Refresh();
					EditorUtility.DisplayDialog("LiveKit FFI", "Downloaded FFI for the current editor platform.", "OK");
				}
				else
				{
					EditorUtility.DisplayDialog("LiveKit FFI", "FFI already present for the current editor platform.", "OK");
				}
			}
			catch (Exception ex)
			{
				EditorUtility.DisplayDialog("LiveKit FFI", $"Failed: {ex.Message}", "OK");
			}
		}

		[MenuItem("Convai/LiveKit/Download FFIs for all platforms", priority = 1001)]
		private static async void MenuDownloadAllFfis()
		{
			try
			{
				int downloads = 0;
				foreach (string platform in new[] { "android", "ios", "macos", "linux", "windows" })
				{
					foreach (string arch in GetArchsForPlatform(platform))
					{
						if (await EnsureFfiInstalledAsync(platform, arch, force: true))
						{
							downloads++;
						}
					}
				}
				AssetDatabase.Refresh();
				EditorUtility.DisplayDialog("LiveKit FFI", $"Downloaded/updated {downloads} FFI packages.", "OK");
			}
			catch (Exception ex)
			{
				EditorUtility.DisplayDialog("LiveKit FFI", $"Failed: {ex.Message}", "OK");
			}
		}

		private static async Task<bool> EnsureEditorFfiInstalledAsync(bool force = false)
		{
			(string platform, string arch, string libName) = DetectEditorTarget();
			if (string.IsNullOrEmpty(platform) || string.IsNullOrEmpty(arch))
			{
				Debug.Log("[LiveKit] Unsupported editor platform/arch for auto-download. Skipping.");
				return false;
			}

			return await EnsureFfiInstalledAsync(platform, arch, force);
		}

		private static async Task<bool> EnsureFfiInstalledAsync(string platform, string arch, bool force)
		{
            string libName = GetLibNameForPlatform(platform);
			if (string.IsNullOrEmpty(libName)) return false;

            string destDir = Path.Combine(_pluginsDir, $"ffi-{platform}-{arch}");
            string destLib = Path.Combine(destDir, libName);

			if (!force && File.Exists(destLib))
			{
				return false; // already present
			}

			Directory.CreateDirectory(_downloadsDir);
			Directory.CreateDirectory(destDir);

            string escapedTag = Uri.EscapeDataString(VERSION_TAG);
            string fileName = $"ffi-{platform}-{arch}.zip";
            string url = $"{RELEASES_BASE_URL}/{escapedTag}/{fileName}";
            string zipPath = Path.Combine(_downloadsDir, fileName);

			Debug.Log($"[LiveKit] Downloading {url} -> {zipPath}");
            string progressTitle = "LiveKit FFI";
            string downloadMessage = $"Downloading {fileName}";
            Progress<float> progress = new Progress<float>(value =>
			{
                float clamped = float.IsNaN(value) ? 0f : Mathf.Clamp01(value);
				EditorUtility.DisplayProgressBar(progressTitle, downloadMessage, clamped);
			});

			try
			{
				await DownloadFileAsync(url, zipPath, progress);
				EditorUtility.DisplayProgressBar(progressTitle, $"Extracting {fileName}", 1f);
				await Task.Run(() => ExtractZip(zipPath, destDir));
			}
			finally
			{
				EditorUtility.ClearProgressBar();
			}

			Debug.Log("[LiveKit] LiveKit FFI ready: " + destLib);
			return true;
		}

		private static (string platform, string arch, string libName) DetectEditorTarget()
		{
			string platform;
			switch (Application.platform)
			{
				case RuntimePlatform.OSXEditor:
					platform = "macos";
					break;
				case RuntimePlatform.WindowsEditor:
					platform = "windows";
					break;
				case RuntimePlatform.LinuxEditor:
					platform = "linux";
					break;
				default:
					return (null, null, null);
			}

			string arch = MapArchitecture();
			string libName = GetLibNameForPlatform(platform);

			return (platform, arch, libName);
		}

		private static string MapArchitecture()
		{
			try
			{
                Architecture arch = RuntimeInformation.ProcessArchitecture;
				return arch switch
				{
					Architecture.Arm64 => "arm64",
					Architecture.X64 => "x86_64",
					_ => "x86_64"
				};
			}
			catch
			{
				// Fallback for older runtimes on mac
				if (Application.platform == RuntimePlatform.OSXEditor)
				{
					// Best-effort: Apple Silicon editors default to arm64 nowadays
					return SystemInfo.processorType.ToLower().Contains("apple") ? "arm64" : "x86_64";
				}
				return Environment.Is64BitProcess ? "x86_64" : "x86_64";
			}
		}

		private static string GetLibNameForPlatform(string platform)
		{
			return platform switch
			{
				"macos" => "liblivekit_ffi.dylib",
				"linux" => "liblivekit_ffi.so",
				"windows" => "livekit_ffi.dll",
				"android" => "liblivekit_ffi.so",
				"ios" => "liblivekit_ffi.a",
				_ => null
			};
		}

		private static string[] GetArchsForPlatform(string platform)
		{
			switch (platform)
			{
				case "android":
					return new[] { "armv7", "arm64", "x86_64" };
				case "ios":
					return new[] { "arm64", "sim-arm64" };
				case "linux":
					return new[] { "x86_64" };
				case "macos":
					return new[] { "arm64", "x86_64" };
				case "windows":
					return new[] { "arm64", "x86_64" };
				default:
					return Array.Empty<string>();
			}
		}

		private static async Task DownloadFileAsync(string url, string outputPath, IProgress<float> progress)
		{
			using HttpResponseMessage response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
			response.EnsureSuccessStatusCode();

            long? contentLength = response.Content.Headers.ContentLength;
			await using Stream downloadStream = await response.Content.ReadAsStreamAsync();
			await using FileStream fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 81920, useAsync: true);

            byte[] buffer = new byte[81920];
			long totalRead = 0;
			progress?.Report(0f);

			while (true)
			{
                int read = await downloadStream.ReadAsync(buffer, 0, buffer.Length);
				if (read == 0)
				{
					break;
				}

				await fileStream.WriteAsync(buffer, 0, read);
				totalRead += read;

				if (contentLength.HasValue && contentLength.Value > 0)
				{
					progress?.Report((float)totalRead / contentLength.Value);
				}
			}

			progress?.Report(1f);
		}

		private static void ExtractZip(string zipPath, string destDir)
		{
            using FileStream fileStream = File.OpenRead(zipPath);
            using ZipArchive archive = new ZipArchive(fileStream, ZipArchiveMode.Read);
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string fullPath = Path.Combine(destDir, entry.FullName);
                string directory = Path.GetDirectoryName(fullPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (string.IsNullOrEmpty(entry.Name))
                {
                    continue; // directory entry
                }

                entry.ExtractToFile(fullPath, overwrite: true);
            }
        }
	}
}
#endif
