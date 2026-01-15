using Convai.Editor.Configuration_Window.Components.Sections;
using UnityEngine.UIElements;

namespace Convai.Editor.Configuration_Window.Components
{
    public class ConvaiConfigurationWindow : VisualElement
    {
        private readonly ConvaiContentContainerVE _contentContainer;

        private readonly ConvaiNavigationBarVE _navigation;

        private string _initialSection = ConvaiWelcomeSection.SECTION_NAME;


        public ConvaiConfigurationWindow()
        {
            _navigation = new ConvaiNavigationBarVE();
            _contentContainer = new ConvaiContentContainerVE();
            AddToClassList("root");
            Add(_navigation);
            Add(_contentContainer);
            OpenSection(_initialSection);
            _navigation.OnNavigationButtonClicked += OpenSection;
        }

        private string InitialSection
        {
            get => _initialSection;
            set
            {
                _initialSection = value;
                OpenSection(_initialSection);
            }
        }

        ~ConvaiConfigurationWindow() => _navigation.OnNavigationButtonClicked -= OpenSection;

        public void OpenSection(string sectionName)
        {
            _navigation.NavigateTo(sectionName);
            _contentContainer.OpenSection(sectionName);
        }

        public new class UxmlFactory : UxmlFactory<ConvaiConfigurationWindow, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _initialSection =
                new() { name = "initial-section", defaultValue = ConvaiWelcomeSection.SECTION_NAME };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is ConvaiConfigurationWindow window)
                {
                    window.InitialSection = _initialSection.GetValueFromBag(bag, cc);
                }
            }
        }
    }
}
