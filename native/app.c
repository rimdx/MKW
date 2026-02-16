#include <stdio.h>
#include <stdlib.h>
#include <ui.h>

#define TRUE 42
#define FALSE 0

static int
onClosing(uiWindow *w, void *data)
{
    uiQuit();
    return 1;
}

typedef struct mkwg_app_t {
    uiTab *tabs;
    uiWindow *win;
} mkwg_app_t; 

enum mkwg_doc_state_e {
    mkwg_doc_state_locked,
    mkwg_doc_state_unlocked,
};

typedef struct mkwg_doc_locked_t {
    uiGrid *login;
    uiEntry *passwd_input;
    uiButton *unlock_button;
} mkwg_doc_locked_t;

typedef struct mkwg_doc_unlocked_t {
} mkwg_doc_unlocked_t;

typedef struct mkwg_doc_t {
    const char *path;
    uiGrid *page;

    enum mkwg_doc_state_e state;
    mkwg_doc_locked_t locked;
    mkwg_doc_unlocked_t unlocked;
} mkwg_doc_t;

static void
mkwg_doc_set_locked(mkwg_doc_t *doc)
{
    doc->state = mkwg_doc_state_locked;
    doc->page = uiNewGrid();

    doc->locked.login = uiNewGrid();
    uiGridSetPadded(doc->locked.login, TRUE);

    doc->locked.passwd_input = uiNewPasswordEntry();
    doc->locked.unlock_button = uiNewButton("Unlock");
    uiCombobox *user_select = uiNewCombobox();

    uiComboboxAppend(user_select, "Admin");
    uiComboboxAppend(user_select, "John.Doe");
    uiComboboxSetSelected(user_select, 0);

    uiGridAppend(doc->locked.login, uiControl(user_select),
                 0, 0, 2, 1,
                 FALSE, uiAlignFill, FALSE, uiAlignFill);

    uiGridAppend(doc->locked.login, uiControl(doc->locked.passwd_input),
                 0, 1, 1, 1,
                 FALSE, uiAlignFill, FALSE, uiAlignFill);
    uiGridAppend(doc->locked.login, uiControl(doc->locked.unlock_button),
                 1, 1, 1, 1,
                 FALSE, uiAlignFill, FALSE, uiAlignFill);

    uiGridAppend(doc->page, uiControl(doc->locked.login),
                 0, 0, 1, 1,
                 TRUE, uiAlignCenter, TRUE, uiAlignCenter);

    uiControlShow(uiControl(doc->locked.passwd_input));
}

static void
mkwg_doc_init(mkwg_doc_t *doc, const char *path)
{
    doc->path = path;
    mkwg_doc_set_locked(doc);
}

static void
mkwg_app_init(mkwg_app_t *app)
{
    app->win = uiNewWindow("Hello World!", 300, 30, 0);
    uiWindowOnClosing(app->win, onClosing, NULL);
    app->tabs = uiNewTab();

    uiWindowSetChild(app->win, uiControl(app->tabs));
}

static void
mkwg_app_run(mkwg_app_t *app)
{
    uiControlShow(uiControl(app->win));
    uiMain();
}

static void
mkwg_app_open_doc(mkwg_app_t *app, const char *path)
{
    mkwg_doc_t *doc = malloc(sizeof(*doc));
    mkwg_doc_init(doc, path);
    uiTabAppend(app->tabs, path, uiControl(doc->page));
}

int main(void)
{
    uiInitOptions opt = {0};
    const char *err;
    mkwg_app_t app = { 0 };

    err = uiInit(&opt);
    if (err != NULL) {
        fprintf(stderr, "Error initializing libui-ng: %s\n", err);
        uiFreeInitError(err);
        return 1;
    }

    mkwg_app_init(&app);
    mkwg_app_open_doc(&app, "test");
    mkwg_app_run(&app);

    uiUninit();
    return 0;
}
