mergeInto(LibraryManager.library, {
  OpenUrlWindow: function (link) {
    var url = Pointer_stringify(link);
    window.open(url);
  },
});