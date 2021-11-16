mergeInto(LibraryManager.library, {
  Play: function () {
    Main && Main.emit("play");
  },
  PlayerDead: function(){
    Main && Main.emit("dead");
  },
  Fullscreen: function() {
    gameInstance && gameInstance.SetFullscreen(1);
  },
  Windowscreen: function() {
    gameInstance && gameInstance.SetFullscreen(0);
  },
});