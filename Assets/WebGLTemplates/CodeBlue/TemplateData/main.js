var Main = {
  __events: [],
  addEventListener: function (eventName, eventHandler) {
    if (
      !this.__events.some(function (e) {
        return e.name === eventName;
      })
    ) {
      this.__events.push({
        name: eventName,
        handler: eventHandler,
      });
    }
  },
  emit: function (eventName) {
    const event = this.__events.find(function (e) {
      return e.name === eventName;
    });
    const args = [].slice.call(arguments);
    event && event.handler.apply(null, args);
  },
};
