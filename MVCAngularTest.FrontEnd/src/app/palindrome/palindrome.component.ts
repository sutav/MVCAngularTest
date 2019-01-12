import { Component, OnInit, ViewChild } from '@angular/core';
import { PalindromeListComponent } from './palindrome-list/palindrome-list.component';
@Component({
  selector: 'app-palindrome',
  templateUrl: './palindrome.component.html',
  styleUrls: ['./palindrome.component.scss']
})
export class PalindromeComponent implements OnInit {
@ViewChild('list') list: PalindromeListComponent;
  constructor() { }

  ngOnInit() {
  }
  reloadPalindromes() {
    this.list.reloadPalindromes();
  }
}
