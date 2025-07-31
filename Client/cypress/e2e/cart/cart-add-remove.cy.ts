describe('Cart Management - Add and Remove Tests', () => {
  
  context('Người dùng đăng nhập thành công vào hệ thống', () => {
    beforeEach(() => {
      cy.login('user@example.com', '123456789');
      cy.wait(500);
      cy.getCookie('accessToken').should('exist');
      cy.getCookie('refreshToken').should('exist');
      cy.getCookie('cartId').should('exist');
    });

    it('Thêm sản phẩm vào giỏ hàng', () => {
      cy.addToCart();
      cy.visit('/cart');
      cy.get('app-cart-item').its('length').should('be.greaterThan', 0);
    });

    it('Xóa sản phẩm khỏi giỏ hàng', () => {
      cy.addToCart();
      cy.visit('/cart');
      cy.get('.pi-trash').click();
      cy.get('app-cart-item').should('not.exist');
      cy.contains('Giỏ hàng của bạn đang trống').should('be.visible');
      cy.reload();
      cy.get('app-cart-item').should('not.exist');
    });
  });

  context('Người dùng chưa đăng nhập vào hệ thống', () => {
    beforeEach(() => {
      cy.clearCookies();
    });

    it('Thêm sản phẩm vào giỏ hàng', () => {
      cy.addToCart();
      cy.wait(500);
      cy.getCookie('cartId').should('exist');
      cy.visit('/cart');
      cy.get('app-cart-item').should('have.length', 1);
    });

    it('Xóa sản phẩm khỏi giỏ hàng', () => {
      cy.addToCart();
      cy.visit('/cart');
      cy.get('.pi-trash').click();
      cy.get('app-cart-item').should('not.exist');
      cy.contains('Giỏ hàng của bạn đang trống').should('be.visible');
      cy.reload();
      cy.get('app-cart-item').should('not.exist');
    });
  });
});
