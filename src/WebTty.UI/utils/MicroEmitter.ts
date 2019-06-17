type Payload = any;
type Listener = any;

export default class MicroEmitter {
  private listeners: { [key: string]: Listener } = {};

  /* Adds a listener function to the specified event. */
  private _addListener(type: string, listener: Listener, once?: boolean): MicroEmitter {
    this.listeners[type] = this.listeners[type] || [];
    this.listeners[type].push({ listener, once });
    return this;
  }

  /* Adds a listener function to the specified event. */
  public addEventListener(type: string, listener: Listener): MicroEmitter {
    return this._addListener(type, listener, false);
  }

  /* Alias of addListener */
  public on(type: string, listener: Listener): MicroEmitter {
    return this.addEventListener(type, listener);
  }

  public addOnceListener(type: string, listener: Listener): MicroEmitter {
    return this._addListener(type, listener, true);
  }

  /* Alias of addOnceListener */
  public once(type: string, listener: Listener): MicroEmitter {
    return this.addOnceListener(type, listener);
  }

  /* Removes a listener function to the specified event. */
  public removeEventListener(type: string, listener: Listener): MicroEmitter {
    if (!this.listeners[type]) {
      return this;
    }
    if (!this.listeners[type].length) {
      return this;
    }
    if (!listener) {
      delete this.listeners[type];
      return this;
    }
    this.listeners[type] = this.listeners[type].filter((_listener: any) => !(_listener.listener === listener));
    return this;
  }

  /* Alias of removeListener */
  public off(type: string, listener: Listener): MicroEmitter {
    return this.removeEventListener(type, listener);
  }

  /*  Emits an specified event. */
  public emit(type: string, payload: Payload): MicroEmitter {
    if (!this.listeners[type]) {
      return this;
    }
    this.listeners[type].forEach((listener: Listener) => {
      listener.listener.apply(this, [payload]);
      if (listener.once) {
        this.removeEventListener(type, listener.listener);
      }
    });
    return this;
  }

  /* Alias of emit */
  public trigger(type: string, payload: Payload): MicroEmitter {
    return this.emit(type, payload);
  }
}
