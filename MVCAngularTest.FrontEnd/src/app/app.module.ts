import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import { PalindromeListComponent } from './palindrome/palindrome-list/palindrome-list.component';
import { PalindromeEditComponent } from './palindrome/palindrome-edit/palindrome-edit.component';
import { PalindromeComponent } from './palindrome/palindrome.component';
import { FormsModule } from '@angular/forms';
@NgModule({
  declarations: [
    AppComponent,
    PalindromeListComponent,
    PalindromeEditComponent,
    PalindromeComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    NgbModule,
    HttpClientModule,
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
