import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environment/environment";
import { LoadingSpinnerService } from "./loading-spinner.service";
import { BehaviorSubject, Observable } from "rxjs";

@Injectable({
    providedIn: 'root'
  })
export class PlayerService {
  private path = environment.apiUrl;

  public remainingTime = new BehaviorSubject<number>(0); // Emits the remaining time in seconds

  constructor(private http: HttpClient, private spinnerService: LoadingSpinnerService) { }

  processChildResponse(prompt: string): Observable<{ reply: string }> {
    return this.http.post<{ reply: string }>(this.path + "Books/ProcessChildResponse", { prompt });
  }


}