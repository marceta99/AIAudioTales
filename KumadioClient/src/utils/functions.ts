import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { Capacitor } from '@capacitor/core';

export function isNativeApp(): boolean {
  return Capacitor.isNativePlatform();
}

export const passwordMatchValidator : ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');
  
    return password && confirmPassword && password.value !== confirmPassword.value
      ? { 'passwordsMissMatch': true }
      : null;
  };