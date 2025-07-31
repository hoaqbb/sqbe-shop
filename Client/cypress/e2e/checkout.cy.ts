describe('Checkout Function Tests', () => {
  context('Người dùng đăng nhập thành công vào hệ thống', () => {
    beforeEach(() => {
      cy.login('user@example.com', '123456789');
      cy.wait(500);
      cy.getCookie('accessToken').should('exist');
      cy.getCookie('refreshToken').should('exist');
      cy.getCookie('cartId').should('exist');
      cy.addToCart();
      cy.visit('/cart');
    });
  });

  context('Người dùng chưa đăng nhập vào hệ thống', () => {
    beforeEach(() => {
      cy.clearCookies();
      cy.addToCart();
      cy.getCookie('cartId').should('exist');
      cy.visit('/cart');
    });

    it('Mua hàng thành công với phương thức Thanh toán khi nhận hàng(COD)', () => {
      cy.fillCheckoutForm(
        'Đình Hoàng',
        'teemod113@gmail.com',
        '0333077198',
        '107/81 TCH 35',
        'Hồ Chí Minh',
        '12',
        'Tân Chánh Hiệp',
        'COD',
        'cod'
      );

      cy.get('#checkout-btn').click();
      cy.intercept('https://localhost:7100/api/Orders').as('createOrder');
      cy.wait('@createOrder').then((interception) => {
        expect(interception.response?.statusCode).to.eq(200);
      });
      cy.url().should('include', '/checkout/result');
      cy.contains('Đặt hàng thành công!').should('be.visible');
    });

    it('Đặt hàng với phương thức Thanh toán bằng VNPay và chuyển hướng tới trang thanh toán của VNPay', () => {
      cy.fillCheckoutForm(
        'Đình Hoàng',
        'teemod113@gmail.com',
        '0333077198',
        '107/81 TCH 35',
        'Hồ Chí Minh',
        '12',
        'Tân Chánh Hiệp',
        'VNPay',
        'vnpay'
      );

      cy.get('#checkout-btn').click();
      cy.intercept('https://localhost:7100/api/Orders').as('createOrder');
      cy.wait('@createOrder').then((interception) => {
        expect(interception.response?.statusCode).to.eq(200);

        const body = interception.response?.body;

        expect(body).to.have.property('success', true);

        if (body.paymentMethod === 'vnpay') {
          expect(body.redirectUrl).to.include('vnpayment.vn');
        } else {
          expect(body.redirectUrl).to.be.undefined;
        }
      });
    });

    it('Đặt hàng với phương thức Thanh toán bằng PayPal và chuyển hướng tới trang thanh toán của PayPal', () => {
      cy.fillCheckoutForm(
        'Đình Hoàng',
        'teemod113@gmail.com',
        '0333077198',
        '107/81 TCH 35',
        'Hồ Chí Minh',
        '12',
        'Tân Chánh Hiệp',
        'PayPal',
        'paypal'
      );

      cy.get('#checkout-btn').click();
      cy.intercept('https://localhost:7100/api/Orders').as('createOrder');
      cy.wait('@createOrder').then((interception) => {
        expect(interception.response?.statusCode).to.eq(200);

        const body = interception.response?.body;

        expect(body).to.have.property('success', true);

        if (body.paymentMethod === 'paypal') {
          expect(body.redirectUrl).to.include('paypal.com');
        } else {
          expect(body.redirectUrl).to.be.undefined;
        }
      });
    });
  });
});
