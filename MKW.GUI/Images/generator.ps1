Get-ChildItem $PSScriptRoot *.xaml | ForEach-Object {
    $name = $_.BaseName

"using System.Windows.Controls;

namespace MKW.GUI.Images
{
    public partial class $name : Viewbox
    {
        public $name()
        {
            InitializeComponent();
        }
    }
}" | Out-File -FilePath "$_.cs" -Encoding utf8BOM

  svn add "$_.cs" --force
}

Write-Host "-----"

svn status $PSScriptRoot
