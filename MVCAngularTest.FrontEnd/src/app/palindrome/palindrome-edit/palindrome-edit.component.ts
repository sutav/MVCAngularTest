import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { PalindromeService } from '../palindrome.service';
import { Subscriber } from 'rxjs';
import { HttpResponse } from '@angular/common/http';
@Component({
  selector: 'app-palindrome-edit',
  templateUrl: './palindrome-edit.component.html',
  styleUrls: ['./palindrome-edit.component.scss'],
  providers: [PalindromeService]
})
export class PalindromeEditComponent implements OnInit {
  palindromeWord: string;  
  @Output()  palindromeVerified: EventEmitter<string>  = new  EventEmitter<string>();
  constructor(private palindromeService: PalindromeService) { 

  }

  ngOnInit() {
  }
  checkPalindrome() {
    if (this.isPalindrome(this.palindromeWord)) {
      this.palindromeService.savePalindrome({ Id: null, PalindromeWord: this.palindromeWord })
      .subscribe(
        (res:any) => {
        alert("this is Palindrome and it will be saved to database");
        this.palindromeVerified.emit(this.palindromeWord);
      },
      (err: any) => {
        console.log(err);
        alert("Error saving data");
        //TODO: Display error message to user
      });
      
    } else {
      alert('This is not Palindrome!');
      this.palindromeWord = '';
    }
  }
  isPalindrome(str) {
    return str == str.split('').reverse().join('');
  }
}
