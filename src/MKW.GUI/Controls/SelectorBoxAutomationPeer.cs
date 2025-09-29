using System.Windows;
using System.Windows.Automation.Peers;

namespace MKW.GUI.Controls
{
    internal sealed class SelectorBoxAutomationPeer : SelectorAutomationPeer
    {
        public SelectorBoxAutomationPeer(SelectorBox owner)
            : base(owner)
        {
        }

        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        {
            return new SelectorBoxItemAutomationPeer(item, this);
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        protected override string GetClassNameCore()
        {
            return nameof(SelectorBox);
        }

        protected override List<AutomationPeer> GetChildrenCore()
        {
            SelectorBox owner = (SelectorBox)Owner;

            List<AutomationPeer> children = [];

            foreach (object? item in owner.Items)
            {
                DependencyObject container = owner.ItemContainerGenerator.ContainerFromItem(item);

                if (container is SelectorBox selectorBox && selectorBox.IsVisible)
                {
                    AutomationPeer peer = FromElement(selectorBox) ?? CreatePeerForElement(selectorBox);

                    if (peer != null)
                    {
                        children.Add(peer);
                    }
                }
            }

            return children;
        }
    }
}
