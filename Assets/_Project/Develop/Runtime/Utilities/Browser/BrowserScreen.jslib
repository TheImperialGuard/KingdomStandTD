var BrowserScreenLibrary = {

  BrowserScreen_GetCanvasClientHeight: function () {
    return Module['canvas'].clientHeight;
  }
};

mergeInto(LibraryManager.library, BrowserScreenLibrary);
