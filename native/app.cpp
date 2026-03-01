#include <stdlib.h>
#include <wx/wx.h>

class mkwg_doc_locked : public wxFrame {

};

class mkwg_doc : public wxPanel {
private:

public:
    mkwg_doc(wxWindow *parent) : wxPanel(parent)
    {
        wxButton *button = new wxButton(this, wxID_ANY, "Unlock");
        wxTextCtrl *passwd_input = new wxTextCtrl(this, wxID_ANY);

        wxBoxSizer *sizer = new wxBoxSizer(wxHORIZONTAL);
        sizer->Add(passwd_input, 2, wxALL | wxCENTRE, 5);
        sizer->Add(button, 0, wxALL | wxCENTRE, 5);
        sizer->SetMinSize(0, 300);
        SetSizer(sizer);
    }
};

class mkwg_mainwin : public wxFrame {
public:
    mkwg_mainwin() : wxFrame(NULL, wxID_ANY, "Multi-Key Wallet") {
        wxMenu *menuFile = new wxMenu;
        menuFile->Append(wxID_EXIT);

        wxMenu *menuHelp = new wxMenu;
        menuHelp->Append(wxID_ABOUT);

        wxMenuBar *menuBar = new wxMenuBar;
        menuBar->Append(menuFile, "&File");
        menuBar->Append(menuHelp, "&Help");

        SetMenuBar(menuBar);

        CreateStatusBar();
        SetStatusText("Welcome to wxWidgets!");

        auto *content = new mkwg_doc(this);
    }
};

class mkwg_app : public wxApp {
   virtual bool OnInit() override
   {
       mkwg_mainwin *win = new mkwg_mainwin();
       win->Show(true);
       return true;
   } 
};

wxIMPLEMENT_APP(mkwg_app);
