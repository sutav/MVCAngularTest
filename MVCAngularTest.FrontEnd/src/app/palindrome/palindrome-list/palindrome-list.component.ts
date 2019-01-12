import { Component, OnInit } from '@angular/core';
import { PalindromeService } from '../palindrome.service';
import { Palindrome } from '../palindrome.model';
@Component({
  selector: 'app-palindrome-list',
  templateUrl: './palindrome-list.component.html',
  styleUrls: ['./palindrome-list.component.scss'],
  providers : [ PalindromeService ]
})
export class PalindromeListComponent implements OnInit {
  palindromeList: Palindrome[] = [];
  constructor(private palindromeServerice: PalindromeService) { }

  ngOnInit() {
    this.palindromeServerice.getPalindromes().subscribe(
      (res: Palindrome[]) => {
        this.palindromeList = res;
  },
  (err: any) => {
      console.log(err);
      //TODO: Display error message to user
  });
}
}
