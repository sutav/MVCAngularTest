import { Injectable } from '@angular/core';
import { Palindrome  } from './palindrome.model';
import { ApiUrls } from '../api-urls';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class PalindromeService {

  constructor(private http: HttpClient ) { }

  getPalindromes(): Observable<Palindrome[]> {
    return this.http.get<Palindrome[]>(ApiUrls.GetPalindromes);
  }
}
