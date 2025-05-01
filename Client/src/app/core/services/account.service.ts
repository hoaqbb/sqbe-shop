import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { tap } from 'rxjs';
import { User } from '../../shared/models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  currentUser = signal<User | null>(null);

  constructor(private http: HttpClient) { }

  register(model: any) {
    return this.http.post(this.baseUrl + '/api/Accounts/register', model).pipe(
      tap((user: User) => this.setCurrentUserSource(user))
    );
  }

  login(model: any) {
    return this.http.post<User>(this.baseUrl + "/api/Accounts/login", model).pipe(
      tap((res: User) => this.setCurrentUserSource(res))
    );
  }

  signInWithGoogle(token: string) {
    return this.http.post<User>(this.baseUrl + "/api/Accounts/login-with-google", {token: token}).pipe(
      tap((res: User) => this.setCurrentUserSource(res))
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
