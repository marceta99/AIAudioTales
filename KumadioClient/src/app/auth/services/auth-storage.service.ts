// src/app/core/services/auth-storage.service.ts
import { Injectable } from '@angular/core';
import { Preferences } from '@capacitor/preferences';
import { Capacitor } from '@capacitor/core';

@Injectable({
  providedIn: 'root'
})
export class AuthStorageService {

  private ACCESS_TOKEN_KEY = 'access_token';
  private REFRESH_TOKEN_KEY = 'refresh_token';

  async setTokens(accessToken: string, refreshToken: string): Promise<void> {
    if (Capacitor.isNativePlatform()) {
      await Preferences.set({ key: this.ACCESS_TOKEN_KEY, value: accessToken });
      await Preferences.set({ key: this.REFRESH_TOKEN_KEY, value: refreshToken });
    }
  }

  async getAccessToken(): Promise<string | null> {
    if (Capacitor.isNativePlatform()) {
      const res = await Preferences.get({ key: this.ACCESS_TOKEN_KEY });
      return res.value;
    }
    return null;
  }

  async getRefreshToken(): Promise<string | null> {
    if (Capacitor.isNativePlatform()) {
      const res = await Preferences.get({ key: this.REFRESH_TOKEN_KEY });
      return res.value;
    }
    return null;
  }

  async clearTokens(): Promise<void> {
    if (Capacitor.isNativePlatform()) {
      await Preferences.remove({ key: this.ACCESS_TOKEN_KEY });
      await Preferences.remove({ key: this.REFRESH_TOKEN_KEY });
    }
  }
}
