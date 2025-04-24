import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from "./features/layout/header/header.component";
import { FooterComponent } from "./features/layout/footer/footer.component";
import { AccountService } from './core/services/account.service';
import { User } from './shared/models/user';
import { ShopService } from './core/services/shop.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, FooterComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit{
  
  constructor(private accountService: AccountService, private shopService: ShopService) { }

  ngOnInit(): void {
    this.setCurrentUser();
    this.getCategories();
  }

  setCurrentUser() {
    const user: User = JSON.parse(localStorage.getItem('user'));

    if(user) {
      this.accountService.currentUser.set(user);
    }
  }

  getCategories() {
    this.shopService.getCategories().subscribe();
  }
}
