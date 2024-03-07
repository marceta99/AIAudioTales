import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environment/environment";
import { LoadingSpinnerService } from "./loading-spinner.service";
import { Basket } from "src/app/entities";
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root'
  })
  export class StripeService {
    private path = environment.apiUrl;

    constructor(private httpClient: HttpClient, private spinnerService: LoadingSpinnerService) { }

    createOrder(basket: Basket): Observable<{sessionId: string}> {       
        return this.httpClient.post<{sessionId: string}>(this.path + "Payment/PlaceOrder", basket, {withCredentials: true});
    }
    
    createSubscribeSession(): Observable<{sessionId: string}> {       
      return this.httpClient.post<{sessionId: string}>(this.path + "Payment/CreateSubscribeSession",{withCredentials: true});
    }
  }