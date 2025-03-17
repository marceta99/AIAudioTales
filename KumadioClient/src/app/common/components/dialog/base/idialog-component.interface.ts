// idialog-component.interface.ts
export interface IDialogComponent<TProps, TResult = any> {
    /**
     * Assigned at runtime by the DialogService.
     * The component calls this to close and optionally emit a result.
     */
    closeDialog: (value?: TResult) => void;
    
    dialogProps: TProps;
  
    /**
     * Add *all* the input fields your dialog needs, or
     * define them in TProps and then implement them here.
     */
    // e.g., if you want them declared individually:
    // title: string;
    // data?: TProps;
  
    // Alternatively, some people prefer to define them
    // as separate fields but the key idea is that TProps
    // is the shape of the props you expect to receive.
}
