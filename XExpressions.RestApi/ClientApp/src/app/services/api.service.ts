import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EvaluateResponseModel } from '../models/EvaluateResponseModel';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  constructor(private http: HttpClient) { }

  evaluate(expression: string): Observable<EvaluateResponseModel> {
    return this.http.post<EvaluateResponseModel>(`/api/evaluate`,
    {
      expression: expression,
      identifiers: [
        {
          name: 'pi',
          value: '3.141592653589793238',
          valueKind: 'decimal'
        }
      ]
    });
  }
}
