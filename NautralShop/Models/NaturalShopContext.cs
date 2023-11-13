using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace NautralShop.Models;

public partial class NaturalShopContext : DbContext
{
    public NaturalShopContext()
    {
    }

    public NaturalShopContext(DbContextOptions<NaturalShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccountType> AccountTypes { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Function> Functions { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<StatusOrder> StatusOrders { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    public virtual DbSet<VoucherDetail> VoucherDetails { get; set; }

    //Add Proc

    //Categories
    public async Task<IList<Category>> GetListCategories()
    {
        return await Categories.FromSqlRaw("EXEC GetListCaterories").ToListAsync();
    }
    public async Task AddCategory(string categoryName)
    {
        await Database.ExecuteSqlRawAsync("EXEC AddCategory @categoryName", new SqlParameter("@categoryName", categoryName));
    }
    public async Task EditCategory(int categoryId, string categoryName)
    {
        await Database.ExecuteSqlRawAsync("EXEC EditCategory @categoryId, @categoryName",
            new SqlParameter("@categoryId", categoryId),
            new SqlParameter("@categoryName",categoryName));
    }
    public async Task DeleteCategory(int categoryId)
    {
        await Database.ExecuteSqlRawAsync("EXEC DeleteCategory @categoryId", new SqlParameter("@categoryId", categoryId));
    }
    //Categories

    //End Add Proc


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=K02\\MSSQLSERVER1;Database=NaturalShop;Trusted_Connection=True;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountType>(entity =>
        {
            entity.HasKey(e => e.AccountTypeId).HasName("PK__AccountT__8F9585AF6EB496B5");

            entity.Property(e => e.AccountTypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0BD9379C87");

            entity.Property(e => e.CategoryName).HasMaxLength(50);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__B611CB7DB397C182");

            entity.Property(e => e.CustomerId)
                .HasMaxLength(100)
                .HasColumnName("customerId");
            entity.Property(e => e.CustomerAddress).HasMaxLength(100);
            entity.Property(e => e.CustomerEmail)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.CustomerPassword).HasMaxLength(200);
            entity.Property(e => e.CustomerPhone)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CustomerUsername)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.AccountType).WithMany(p => p.Customers)
                .HasForeignKey(d => d.AccountTypeId)
                .HasConstraintName("FK_Customers_Functions");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F11D77D51A7");

            entity.Property(e => e.EmployeeId).HasMaxLength(100);
            entity.Property(e => e.EmployeeAddress).HasMaxLength(100);
            entity.Property(e => e.EmployeeEmail)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeName).HasMaxLength(100);
            entity.Property(e => e.EmployeePassword).HasMaxLength(200);
            entity.Property(e => e.EmployeePhone)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.EmployeeUsername)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.AccountType).WithMany(p => p.Employees)
                .HasForeignKey(d => d.AccountTypeId)
                .HasConstraintName("FK_employees_Functions");
        });

        modelBuilder.Entity<Function>(entity =>
        {
            entity.HasKey(e => e.FunctionId).HasName("PK__Function__31ABFAF84734C641");

            entity.Property(e => e.FunctionIcon).HasMaxLength(100);
            entity.Property(e => e.FunctionName).HasMaxLength(50);
            entity.Property(e => e.RouteAction).HasMaxLength(50);
            entity.Property(e => e.RouteArea).HasMaxLength(50);
            entity.Property(e => e.RouteController).HasMaxLength(50);

            entity.HasOne(d => d.AccountType).WithMany(p => p.Functions)
                .HasForeignKey(d => d.AccountTypeId)
                .HasConstraintName("FK_Functions_AccountTypes");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCFA2685AC7");

            entity.Property(e => e.OrderId).HasMaxLength(100);
            entity.Property(e => e.CustomerId).HasMaxLength(100);
            entity.Property(e => e.EmployeeId).HasMaxLength(100);
            entity.Property(e => e.OrderCustomerAddress).HasMaxLength(100);
            entity.Property(e => e.OrderCustomerName).HasMaxLength(100);
            entity.Property(e => e.OrderCustomerPhone)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.OrderDate).HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Orders_Customers");

            entity.HasOne(d => d.Employee).WithMany(p => p.Orders)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_Orders_Employees");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentMethodId)
                .HasConstraintName("FK_Orders_PaymentMethods");

            entity.HasOne(d => d.StatusOrder).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusOrderId)
                .HasConstraintName("FK_Orders_StatusOrder");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D36C31802E0C");

            entity.Property(e => e.OrderDetailId).HasMaxLength(100);
            entity.Property(e => e.OrderId).HasMaxLength(100);
            entity.Property(e => e.ProductId).HasMaxLength(100);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderDetails_Orders");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_OrderDetails_Products");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PK__PaymentM__DC31C1D3E7C49212");

            entity.Property(e => e.PaymentMethodName).HasMaxLength(50);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6CD3094A4D1");

            entity.Property(e => e.ProductId).HasMaxLength(100);
            entity.Property(e => e.ProductImage).HasMaxLength(50);
            entity.Property(e => e.ProductName).HasMaxLength(50);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Products_Categories");
        });

        modelBuilder.Entity<StatusOrder>(entity =>
        {
            entity.HasKey(e => e.StatusOrderId).HasName("PK__StatusOr__6435B6A5237A8BB5");

            entity.ToTable("StatusOrder");

            entity.Property(e => e.StatusOrderName).HasMaxLength(50);
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.VoucherId).HasName("PK__Vouchers__3AEE7921A05C9ECF");

            entity.Property(e => e.VoucherId).HasMaxLength(100);
            entity.Property(e => e.DateExpire).HasColumnType("datetime");
            entity.Property(e => e.EmployeeId).HasMaxLength(100);
            entity.Property(e => e.VoucherName).HasMaxLength(50);

            entity.HasOne(d => d.Employee).WithMany(p => p.Vouchers)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_Vouchers_Employees");
        });

        modelBuilder.Entity<VoucherDetail>(entity =>
        {
            entity.HasKey(e => e.VoucherDetailId).HasName("PK__VoucherD__3B8B7CA01C16E290");

            entity.Property(e => e.VoucherDetailId).HasMaxLength(100);
            entity.Property(e => e.CustomerId).HasMaxLength(100);
            entity.Property(e => e.VoucherId).HasMaxLength(100);

            entity.HasOne(d => d.Customer).WithMany(p => p.VoucherDetails)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_VoucherDetails_Customers");

            entity.HasOne(d => d.Voucher).WithMany(p => p.VoucherDetails)
                .HasForeignKey(d => d.VoucherId)
                .HasConstraintName("FK_VoucherDetails_Vouchers");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
