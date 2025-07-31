describe('Register Function Tests', () => {
    it('Đăng ký thành công với thông tin hợp lệ', () => {
        cy.register('Nguyễn Đình', 'Hoàng', '21/09/2003', 'hoakhanhlunlun@gmail.com', '0994319247', '0994319247');
        cy.wait(500);
        cy.url().should('include', '/account/login');
        cy.contains('Đăng ký tài khoản thành công! Vui lòng kiểm tra email của bạn để kích hoạt tài khoản.')
            .should('be.visible');
    });

    it('Đăng ký thất bại với Email đã tồn tại', () => {
    cy.register('Nguyễn Đình', 'Hoàng', '21/09/2003', 'user@example.com', '0994319247', '0994319247');

    cy.contains('*Email này đã được sử dụng!').should('be.visible');
  });
})