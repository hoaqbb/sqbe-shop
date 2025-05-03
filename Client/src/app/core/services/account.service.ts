import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { tap } from 'rxjs';
import { User } from '../../shared/models/user';
import { CartService } from './cart.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  currentUser = signal<User | null>(null);

  constructor(private http: HttpClient, private cartService: CartService) { }

  register(model: any) {
    return this.http.post(this.baseUrl + '/api/Accounts/register', model).pipe(
      tap((user: User) => {
        this.setCurrentUserSource(user);
        this.cartService.getCart().subscribe();
      })
    );
  }

  login(model: any) {
    return this.http.post<User>(this.baseUrl + "/api/Accounts/login", model).pipe(
      tap((res: User) => {
        this.setCurrentUserSource(res);
        this.cartService.getCart().subscribe();
      })
    );
  }

  signInWithGoogle(token: string) {
    return this.http.post<User>(this.baseUrl + "/api/Accounts/login-with-google", {token: token}).pipe(
      tap((res: User) => {
        this.setCurrentUserSource(res);
        this.cartService.getCart().subscribe();
      })
    );
  }

  setCurrentUserSource(user: User) {
    this.currentUser.set(user);
    localStorage.setItem('user', JSON.stringify(user));
  }

  logout() {
    return this.http.post(this.baseUrl + "/api/Accounts/logout", {}).pipe(
      tap(() => {
        this.removeCurrentUserSource();
      }
    ));
  }
  
  refreshToken() {
    return this.http.post(this.baseUrl + "/api/Accounts/refresh-token", {})
  }

  removeCurrentUserSource() {
    this.currentUser.set(null);
    localStorage.removeItem('user');
  }

  likeProduct(productId: string) {
    return this.http.post(this.baseUrl + "/api/Accounts/like-product/" + productId, {});
  }

  unlikeProduct(productId: string) {
    return this.http.delete(this.baseUrl + "/api/Accounts/unlike-product/" + productId, {});
  }
}
