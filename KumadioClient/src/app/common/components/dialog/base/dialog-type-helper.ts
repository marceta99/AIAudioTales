import { IDialogComponent } from "./idialog-component.interface";

/** Extracts the Props type from a component implementing IDialogComponent<TProps, TResult> */
export type ExtractDialogProps<T> = T extends IDialogComponent<infer Props, any>
? Props
: never;

/** Extracts the Result type from a component implementing IDialogComponent<TProps, TResult> */
export type ExtractDialogResult<T> = T extends IDialogComponent<any, infer R>
? R
: never;