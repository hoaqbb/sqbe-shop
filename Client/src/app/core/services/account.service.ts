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

  setCurrentUserSource(user: User) {
    this.currentUser.set(user);
    localStorage.setItem('user', JSON.stringify(user));
  }
}
