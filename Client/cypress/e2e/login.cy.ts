describe('Login Function Tests', () => {
  it('Đăng nhập thành công với tài khoản hợp lệ', () => {
    cy.login('user@example.com', '123456789')

    cy.url().should('include', '/');
    cy.wait(500);
    cy.getCookie('accessToken').should('exist');
    cy.getCookie('refreshToken').should('exist');
  });

  it('Đăng nhập thất bại với email không hợp lệ', () => {
    cy.login('user@example.com', '1234567890')

    cy.contains('Email hoặc mật khẩu không hợp lệ!').should('be.visible');
  });

  it('Đăng nhập thất bại với mật khẩu không hợp lệ', () => {
    cy.login('fakeemail@example.com', '123456789')

    cy.contains('Email hoặc mật khẩu không hợp lệ!').should('be.visible');
  });

  it('Đăng nhập thất bại vì email chưa được xác thực', () => {
    cy.login('adminn@gmail.com', '123456789');

    cy.contains('Vui lòng xác thực email để kích hoạt tài khoản.').should('be.visible');
  });
});
