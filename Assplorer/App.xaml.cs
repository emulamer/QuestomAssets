using Avalonia;
using Avalonia.Markup.Xaml;

namespace Assplorer
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
