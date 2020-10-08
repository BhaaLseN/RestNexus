/** Defers execution of a callback by a specified number of milliseconds
* @param waitTimeMS A positive number of milliseconds of which to defer execution by
* @param callback A callback to be invoked when the specified wait time has passed
*/
declare function defer(waitTimeMS: number, callback: () => void): void;
