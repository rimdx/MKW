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

    (Get-Content -Path $_).Replace("<Viewbox Width=", "<Viewbox x:Class=`"MKW.GUI.Images.$name`" Width=") | Out-File -FilePath $_ -Encoding ansi

    svn add "$_.cs" --force
}

Write-Host "-----"

svn status $PSScriptRoot
