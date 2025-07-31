describe('Manage Order Function Tests', () => {
  it('Truy cập trang admin thất bại với tài khoản không phải admin', () => {
    cy.login('user@example.com', '123456789');
    cy.wait(500);
    cy.getCookie('accessToken').should('exist');
    cy.getCookie('refreshToken').should('exist');
    cy.visit('/admin/order');

    cy.url().should('include', 'not-found');
  });

  context('Người dùng đăng nhập vào hệ thống với tài khoản admin', () => {
    beforeEach(() => {
      cy.login('admin@gmail.com', 'P@ssWord123');
      cy.wait(500);
      cy.getCookie('accessToken').should('exist');
      cy.getCookie('refreshToken').should('exist');
      cy.visit('/admin/order');
      cy.url().should('include', '/admin/order');
    });

    it('Xem danh sách đơn hàng và lọc đơn hàng', () => {
        cy.get('.filter-btn').click();
        cy.get('select#sort').select('dateAsc');
        cy.get('select#status').select('CHỜ XÁC NHẬN');
        cy.get('select#paymentMethod').select('COD');
        cy.get('.apply-btn').click();
    });

    it('Xem chi tiết đơn hàng', () => {
        cy.get('table>tbody>tr').first().click();
    });

    it('Cập nhật trạng thái đơn hàng', () => {
        cy.get('table>tbody>tr').first().click();
        cy.get('select#orderStatusSelect').scrollIntoView().select('Đang giao hàng');
        cy.get('.btn-success').click();
        cy.wait(500);
        cy.contains('Cập nhật trạng thái đơn hàng thành công.').should('be.visible');
    });
  });
});
