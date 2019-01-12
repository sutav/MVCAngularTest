import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PalindromeListComponent } from './palindrome-list/palindrome-list.component';
import { PalindromeEditComponent } from './palindrome-edit/palindrome-edit.component';
import { PalindromeComponent } from './palindrome.component';
import { PalindromeService } from './palindrome.service';
import { PalindromeRoutingModule } from './palindrome-routing.module';
@NgModule({
  declarations: [PalindromeListComponent, PalindromeEditComponent, PalindromeComponent],
  imports: [
    CommonModule, PalindromeRoutingModule
  ],
  providers : [ PalindromeService ]
})
export class PalindromeModule { }
