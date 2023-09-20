mergeInto(LibraryManager.library, {
    ShowMessage: function (message) {
        window.alert(UTF8ToString(message))
    }
});