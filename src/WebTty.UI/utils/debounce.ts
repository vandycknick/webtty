const debounce = (callback: any, time: number) => {
    let handle: any;
    return (...args: any[]) => {
        clearTimeout(handle);
        handle = setTimeout(() => {
            handle = null;
            callback(...args);
        }, time);
    };
};

export default debounce
