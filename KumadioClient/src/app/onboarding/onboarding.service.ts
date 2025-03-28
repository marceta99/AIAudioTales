// onboarding.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ChildInfo {
  childAge?: number;
  childGender?: string;
  childInterests?: string[];
  favoriteCharacters?: string[];
  improvementAreas?: string[];
  preferredDuration?: string;
}

@Injectable()
export class OnboardingService {
  private baseUrl = 'http://localhost:5000/api/auth';

  constructor(private http: HttpClient) {}

  submitOnboardingData(data: ChildInfo): Observable<any> {
    return this.http.post(`${this.baseUrl}/onboarding`, data, {
      withCredentials: true
    });
  }
}
