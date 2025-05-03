import { computed, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { Cart } from '../../shared/models/cart';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  baseUrl = environment.apiUrl;
  cart = signal<Cart | null>(null);
  count = signal<number>(0);
  amount = signal<number>(0);

  constructor(private http: HttpClient) { }

  getCart() {
    return this.http.get<Cart>(this.baseUrl + '/api/Carts').pipe(
      tap((cart) => {
          this.cart.set(cart);
          this.itemCount();
          this.calculateAmount();
      })
    )
  }

  itemCount(){
    let itemCount = computed(() => {
      return this.cart()?.cartItems.reduce((sum, item) => sum + item.quantity, 0);
    })
    this.count.set(itemCount())
  }

  calculateAmount() {
    let calculate = computed(() => {
      return this.cart()?.cartItems.reduce((amount, item) => {
        if(item.discount > 0) {
          amount += item.price * item.quantity * (1 - item.discount / 100);
        }
        else {
          amount += item.price * item.quantity;
        }
        
        return amount;
      }, 0)
    })
    this.amount.set(calculate());
  }
}
