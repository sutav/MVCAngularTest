import { Injectable } from '@angular/core';
import { Palindrome  } from './palindrome.model';
import { ApiUrls } from '../api-urls';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HttpResponseData } from './response.data';
@Injectable()
export class PalindromeService {

  constructor(private http: HttpClient ) { }

  getPalindromes(): Observable<Palindrome[]> {
    return this.http.get<Palindrome[]>(ApiUrls.GetPalindromes);
  }
  savePalindrome(palindrome: Palindrome): Observable<HttpResponseData> {
    const headers = new HttpHeaders().set('Content-Type', 'application/json');
    return this.http.post<HttpResponseData>(ApiUrls.SavePalindrome, palindrome, {headers: headers});   
  }
}
