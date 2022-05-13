--товары
create table article
(ar_id int identity(1,1),
 ar_name nvarchar(256),
 constraint pk_article primary key (ar_id)
 )
go	 
 
 --аптеки
create table pharmacy
	(ph_id int identity(1,1),
	ph_name nvarchar(256),
	ph_address nvarchar(256),
	ph_phone nvarchar(16),
	constraint pk_pharmacy primary key (ph_id)
	)
go
  
--склады
create table stock
	(st_id int identity(1,1),
	st_ph int not null,
	st_name nvarchar(256),
	constraint pk_stock primary key (st_id),
	constraint fk_stock foreign key (st_ph) references pharmacy(ph_id)
	)
go
   
--партии   
create table batch
   (bt_id int identity(1,1),
    bt_ar int not null,
    bt_st int not null,
    bt_num int,
    constraint pk_batch primary key (bt_id),
    constraint fk_batch_article foreign key (bt_ar) references article(ar_id),
    constraint fk_batch_stock foreign key (bt_st) references stock(st_id)
    )
go    

--партии, представление
create view v_batch
as
	select
		bt_id,
		bt_ar,
		bt_st,
		a.ar_name,
		s.st_name,
		bt_num
		 
    from
		batch b inner join article a on b.bt_ar = a.ar_id
		inner join stock s on b.bt_st = s.st_id

go
    
--количество товаров в аптеке    
create view v_article_total
as    
	select
	a.ar_id,
	a.ar_name,
	s.st_ph ph_id,
	sum(b.bt_num) total
	from article a(nolock) inner join batch b(nolock)  on a.ar_id = b.bt_ar
	inner join stock s(nolock) on b.bt_st = s.st_id   
	group by 
	a.ar_id,
	a.ar_name,
	s.st_ph
go

--добавление товара
create procedure p_article_add 
	@ar_name nvarchar(256)
as
	insert into article (ar_name)
	values (@ar_name)
	select * from article where ar_id = @@identity
go		


--удаление товара
create procedure p_article_del
	@ar_id int
as
	delete from batch where bt_ar = @ar_id
	delete from article where ar_id = @ar_id
go	 		 

--добавление аптеки
create procedure p_pharmacy_add
	@ph_name nvarchar(256),
	@ph_address nvarchar(256),
	@ph_phone nvarchar(16)
as
	insert into pharmacy (ph_name, ph_address, ph_phone)	
	values  (@ph_name, @ph_address, @ph_phone)
	select * from pharmacy where ph_id = @@identity
go	


--удаление аптеки
create procedure p_pharmacy_del
	@ph_id int
as
	delete from batch where bt_st in (select st_id from stock where st_ph = @ph_id)
	delete from stock where st_ph = @ph_id
	delete from pharmacy where ph_id = @ph_id
go		

create procedure p_stock_add
	@st_ph int,
	@st_name nvarchar(256)
as
	insert into stock (st_ph, st_name)
	values (@st_ph, @st_name)	
	select * from stock where st_id = @@identity
go	



create procedure p_stock_del
	@st_id int
as
	delete from batch where bt_st = @st_id
	delete from stock where st_id = @st_id
go	



create procedure p_batch_add
    @bt_ar int,
    @bt_st int,
    @bt_num int
as
	insert into batch (bt_ar, bt_st, bt_num)
	values (@bt_ar, @bt_st, @bt_num)
	select * from v_batch where bt_id = @@identity
go	


create procedure p_batch_id
    @bt_id int
as
	delete from batch where bt_id = @bt_id
go	

create procedure p_total_articles
	@ph_id int
as	
	select * from v_article_total
	where ph_id = @ph_id
	order by ar_name
go	

