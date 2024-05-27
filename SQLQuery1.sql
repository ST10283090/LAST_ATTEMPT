INSERT INTO Products (Prod_Name, Prod_Description, Price, Prod_Availability, Category, CreatedAt, UpdatedAt)
VALUES ('Laminated Steel forged Katana', 
        'Discover the dazzling danger of our handcrafted katana. Each blade embodies centuries of tradition, crafted by and wielded by learned gurus. Holding one means holding a warrior''s spirit, steeped in honor and legacy. It''s more than a sword, it''s a symbol of power and tradition.',
        3600.00,
        1, 
        'Weapons',
        GETDATE(),
        GETDATE()); 


INSERT INTO Products (Prod_Name, Prod_Description, Price, Prod_Availability, Category, CreatedAt, UpdatedAt)
VALUES ('Stone forged "Big Fred Gnome"',
        'Immerse yourself in the charm and craftsmanship of our stone-forged "Big Fred" Gnome sculpture. Every groove etched with the care and skill. Experience the "Medusa" like effect of the capture our art has on the viewer. It''s more than just a gnome, its art, meant to elevate.''',
        2200.00,
        1,
        'Sculptures',
        GETDATE(),
        GETDATE());


INSERT INTO Products (Prod_Name, Prod_Description, Price, Prod_Availability, Category, CreatedAt, UpdatedAt)
VALUES ('KwaNogqaza',
        'Art may be subjective, but the beauty of this art piece is the only objective truth. Inspired by "The place of the tall one, this piece wishes to transport the viewer to somewhere they wish, but have never been to, It''s not just art, its life.',
        1200.00,
        1, 
        'Paintings',
        GETDATE(),
        GETDATE()); 

CREATE TABLE Orders (
    Id INT PRIMARY KEY,
    UserId NVARCHAR(450) NOT NULL, 
    OrderDate DATETIME NOT NULL,
    TotalAmount DECIMAL(10, 2) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    CONSTRAINT FK_Orders_AspNetUsers FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);


