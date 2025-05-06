import { computed, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { Cart, CartItem } from '../../shared/models/cart';
import { HttpClient } from '@angular/common/http';
import { catchError, of, tap, throwError } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  baseUrl = environment.apiUrl;
  cart = signal<Cart>(new Cart());
  count = signal<number>(0);
  amount = signal<number>(0);

  constructor(private http: HttpClient, private toastr: ToastrService) { }

  getCart() {
    return this.http.get<Cart>(this.baseUrl + '/api/Carts').pipe(
      tap((cart) => {
          this.cart.set(cart);
          this.itemCount();
          this.calculateAmount();
      })
    )
  }

  addToCart(productVariantId: number) {
    return this.http.post(this.baseUrl + '/api/Carts/add-item', { productVariantId: productVariantId });
  }

  addToCartAndUpdate(productVariantId: number) {
    const cart = this.cart();
    const existingItem = cart.cartItems.find(x => x.productVariant.id === productVariantId);

    // Nếu đã tồn tại và đạt giới hạn tồn kho
    if (existingItem && existingItem.quantity >= existingItem.productVariant.quantity) {
      this.toastr.warning("Sản phẩm đã đạt tới giới hạn trong kho!");
      return of();
    }

    return this.addToCart(productVariantId).pipe(
      tap((updatedItem: CartItem) => {
        this.cart.update(cart => {
          const existingIndex = cart.cartItems.findIndex(item => item.productVariant.id === productVariantId);
          if (existingIndex !== -1) {
            cart.cartItems[existingIndex] = updatedItem; // replace
          } else {
            cart.cartItems.push(updatedItem);
          }
          this.itemCount();
          this.calculateAmount();
          return cart;
        }),
        catchError((error) => {
          this.toastr.error("Không thể thêm sản phẩm vào giỏ hàng.");
          return throwError(() => error);
        })
      })
    );
  }

  updateCartItem(cartItemId: number, quantity: number) {
    return this.http.put(this.baseUrl + '/api/Carts/items/' + cartItemId, { quantity: quantity }).pipe(
      tap(() => {
        this.itemCount();
        this.calculateAmount();
      })
    );
  }

  removeCartItem(cartItemId: number){
    return this.http.delete(this.baseUrl + '/api/Carts/items/' + cartItemId, {}).pipe(
      tap(() => {
        this.cart.update(cart => {
          cart.cartItems = cart.cartItems.filter(item => item.id !== cartItemId);
          this.itemCount();
          this.calculateAmount()
          return cart;
        });
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
