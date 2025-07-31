describe('Cart Management - Update Tests', () => {
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

    it('Cập nhật hợp lệ: nhập 3', () => {
      cy.get('p-inputnumber input').first().clear().type('3').blur();
      cy.get('p-inputnumber input').first().should('have.value', '3');
    });

    it('Cập nhật âm: -1 -> về 1', () => {
      cy.get('p-inputnumber input').first().clear().type('-1').blur();
      cy.get('p-inputnumber input').first().should('have.value', '1');
    });

    it('Cập nhật là 0 -> về 1', () => {
      cy.get('p-inputnumber input').first().clear().type('0').blur();
      cy.get('p-inputnumber input').first().should('have.value', '1');
    });

    it('Cập nhật lớn hơn tồn kho: 10000', () => {
      cy.get('p-inputnumber input').first().clear().type('10000').blur();
      cy.get('p-inputnumber input').first().should('not.equal', '10000');
    });

    it('Nhập chữ: abc -> về 1', () => {
      cy.get('p-inputnumber input').first().clear().type('abc').blur();
      cy.get('p-inputnumber input').first().should('have.value', '1');
    });
  });

  context('Người dùng chưa đăng nhập vào hệ thống', () => {
    beforeEach(() => {
      cy.clearCookies();
      cy.addToCart();
      cy.visit('/cart');
    });

    it('Cập nhật hợp lệ: nhập 3', () => {
      cy.get('p-inputnumber input').clear().type('3').blur();
      cy.get('p-inputnumber input').should('have.value', '3');
    });

    it('Cập nhật âm: -1 -> về 1', () => {
      cy.get('p-inputnumber input').clear().type('-1').blur();
      cy.get('p-inputnumber input').should('have.value', '1');
    });

    it('Cập nhật là 0 -> về 1', () => {
      cy.get('p-inputnumber input').clear().type('0').blur();
      cy.get('p-inputnumber input').should('have.value', '1');
    });

    it('Cập nhật lớn hơn tồn kho: 10000', () => {
      cy.get('p-inputnumber input').clear().type('10000').blur();
      cy.get('p-inputnumber input').should('not.equal', '10000');
    });

    it('Nhập chữ: abc -> về 1', () => {
      cy.get('p-inputnumber input').clear().type('abc').blur();
      cy.get('p-inputnumber input').should('have.value', '1');
    });
  });
});
