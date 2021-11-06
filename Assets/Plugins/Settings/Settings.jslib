mergeInto(LibraryManager.library, {

  Play: function () {
    Main && Main.emit("play");
  },
  PlayerDead: function(){
    Main && Main.emit("dead");
  },
});