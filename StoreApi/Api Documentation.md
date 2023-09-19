# Api

## User/Shop

### Login
Pass JSON body
- phoneNumber
- password
You will receive a token and can use it for other actions
```
/api/Login/{User/Shop}Login
```

### Get User/Shop [Get]
Pass Authorization token in the header as Bearer it will identify
```
/api/Login/Get{User/Shop}
```

### Register User/Shop [Post]
Pass JSON body of the Shop/User object from the model folder
```
/api/Login/Register{User/Shop}
```

### Edit User/Shop [Put]
Pass token and JSON body of the changed Shop/User object. 
No need to pass Id but pass a random password(it wont be changed)
```
/api/Login/Edit{User/Shop}
```

### Admin login
For Editing and deleting products

## Products

### Get products (pagination enabled)
Get 10 products for each page
```
/api/Products?page={}
```

### Get single product
Get product when an id is passed
```
/api/Products/{id}
```

### Edit Product (admin only)
Pass product obj
```
/api/Products/{id}
```

### Add product
Pass ProductDTO obj, image in base64 encoding
```
/api/Products
```

### Find product (for shops)
Pass ProductSearchParamsShop DTO object
```
/api/Products/Find
```

## User address
Pass token for all these requests

### Get all user addresses
```
/api/UserAddresses
```

### Get single address
```
/api/UserAddresses/{id}
```

### Edit address
Pass address obj
```
/api/UserAddresses
```

### Add address
Pass address obj
```
/api/UserAddresses
```

### Delete Address
Pass id from formdata
```
/api/UserAddress
```

## Shop Addresses
Pass token for all its requests

### Get address
```
/api/ShopAddresses
```

### Edit address
Pass address obj
```
/api/ShopAddresses
```

### Add address
Can add only one address
```
/api/ShopAddresses
```

### Delete address
Pass addressId as id from formdata
```
/api/ShopAddress
```