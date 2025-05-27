// onboarding.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { OnboardingDto, OnboardingQuestionDto } from '../entities';

@Injectable()
export class OnboardingService {
  private baseUrl = 'http://localhost:5000/api/auth';

  constructor(private http: HttpClient) {}

  getQuestions(): Observable<OnboardingQuestionDto[]> {
    return this.http.get<OnboardingQuestionDto[]>(`${this.baseUrl}/onboarding-questions`, { withCredentials: true });
  }

  complete(data: OnboardingDto): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/complete`, data, { withCredentials: true });
  }
}
