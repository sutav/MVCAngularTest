
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PalindromeComponent } from './palindrome.component';
const routes: Routes = [
    {
        path: 'palindrpme',
        children: []
    }
];

@NgModule({
    imports: [RouterModule.forChild([
        { path: 'Palindrome', component: PalindromeComponent }
    ])
    ],
    exports: [RouterModule]
})
export class PalindromeRoutingModule { }