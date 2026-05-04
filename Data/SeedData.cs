using FoodRoute.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodRoute.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        // Check if already seeded
        if (context.Restaurants.Any())
        {
            return;
        }

        // =============================================
        // ISTANBUL RESTAURANTS (8 adet)
        // =============================================
        var istanbul1 = new Restaurant
        {
            Name = "Nusr-Et Steakhouse",
            City = "İstanbul",
            Address = "Etiler, Nispetiye Cad. No:87, Beşiktaş",
            Latitude = 41.0812,
            Longitude = 29.0321,
            LocationUrl = "https://maps.google.com/?q=41.0812,29.0321"
        };

        var istanbul2 = new Restaurant
        {
            Name = "Mikla Restaurant",
            City = "İstanbul",
            Address = "Beyoğlu, The Marmara Pera, Meşrutiyet Cad. No:15",
            Latitude = 41.0305,
            Longitude = 28.9766,
            LocationUrl = "https://maps.google.com/?q=41.0305,28.9766"
        };

        var istanbul3 = new Restaurant
        {
            Name = "Van Kahvaltı Evi",
            City = "İstanbul",
            Address = "Cihangir, Defterdar Yokuşu No:52, Beyoğlu",
            Latitude = 41.0317,
            Longitude = 28.9848,
            LocationUrl = "https://maps.google.com/?q=41.0317,28.9848"
        };

        var istanbul4 = new Restaurant
        {
            Name = "Karaköy Güllüoğlu",
            City = "İstanbul",
            Address = "Karaköy, Kemankeş Cad. No:83-85",
            Latitude = 41.0229,
            Longitude = 28.9744,
            LocationUrl = "https://maps.google.com/?q=41.0229,28.9744"
        };

        var istanbul5 = new Restaurant
        {
            Name = "Çiya Sofrası",
            City = "İstanbul",
            Address = "Kadıköy, Güneşlibahçe Sok. No:44",
            Latitude = 40.9907,
            Longitude = 29.0254,
            LocationUrl = "https://maps.google.com/?q=40.9907,29.0254"
        };

        var istanbul6 = new Restaurant
        {
            Name = "Kronotrop Coffee",
            City = "İstanbul",
            Address = "Cihangir, Firuzağa Mah. Cihangir Cad. No:2",
            Latitude = 41.0319,
            Longitude = 28.9821,
            LocationUrl = "https://maps.google.com/?q=41.0319,28.9821"
        };

        var istanbul7 = new Restaurant
        {
            Name = "Kanaat Lokantası",
            City = "İstanbul",
            Address = "Üsküdar, Selmanipak Cad. No:9",
            Latitude = 41.0234,
            Longitude = 29.0156,
            LocationUrl = "https://maps.google.com/?q=41.0234,29.0156"
        };

        var istanbul8 = new Restaurant
        {
            Name = "Baylan Pastanesi",
            City = "İstanbul",
            Address = "Kadıköy, Muvakkithane Cad. No:19",
            Latitude = 40.9901,
            Longitude = 29.0234,
            LocationUrl = "https://maps.google.com/?q=40.9901,29.0234"
        };

        // =============================================
        // ANKARA RESTAURANTS (6 adet)
        // =============================================
        var ankara1 = new Restaurant
        {
            Name = "Trilye Restaurant",
            City = "Ankara",
            Address = "Çankaya, Aşağı Ayrancı, Hoşdere Cad. No:6",
            Latitude = 39.9042,
            Longitude = 32.8564,
            LocationUrl = "https://maps.google.com/?q=39.9042,32.8564"
        };

        var ankara2 = new Restaurant
        {
            Name = "Hacı Arif Bey Kebapçısı",
            City = "Ankara",
            Address = "Kavaklıdere, Tunalı Hilmi Cad. No:114",
            Latitude = 39.9087,
            Longitude = 32.8621,
            LocationUrl = "https://maps.google.com/?q=39.9087,32.8621"
        };

        var ankara3 = new Restaurant
        {
            Name = "Sedef Serpme Kahvaltı",
            City = "Ankara",
            Address = "Çankaya, Kireçburnu Cad. No:22",
            Latitude = 39.9056,
            Longitude = 32.8601,
            LocationUrl = "https://maps.google.com/?q=39.9056,32.8601"
        };

        var ankara4 = new Restaurant
        {
            Name = "Günaydın Kasap & Steakhouse",
            City = "Ankara",
            Address = "GOP, Tahran Cad. No:8/A",
            Latitude = 39.9012,
            Longitude = 32.8587,
            LocationUrl = "https://maps.google.com/?q=39.9012,32.8587"
        };

        var ankara5 = new Restaurant
        {
            Name = "D'oreille Patisserie",
            City = "Ankara",
            Address = "Çankaya, Arjantin Cad. No:24",
            Latitude = 39.9078,
            Longitude = 32.8634,
            LocationUrl = "https://maps.google.com/?q=39.9078,32.8634"
        };

        var ankara6 = new Restaurant
        {
            Name = "Vegan Mutfak",
            City = "Ankara",
            Address = "Bahçelievler, 7. Cadde No:45",
            Latitude = 39.9134,
            Longitude = 32.8234,
            LocationUrl = "https://maps.google.com/?q=39.9134,32.8234"
        };

        // =============================================
        // IZMIR RESTAURANTS (6 adet)
        // =============================================
        var izmir1 = new Restaurant
        {
            Name = "Sakız Restaurant",
            City = "İzmir",
            Address = "Alsancak, Şehit Nevres Bey Bulvarı No:8",
            Latitude = 38.4352,
            Longitude = 27.1438,
            LocationUrl = "https://maps.google.com/?q=38.4352,27.1438"
        };

        var izmir2 = new Restaurant
        {
            Name = "Deniz Restaurant",
            City = "İzmir",
            Address = "Kordon, Atatürk Cad. No:188",
            Latitude = 38.4312,
            Longitude = 27.1352,
            LocationUrl = "https://maps.google.com/?q=38.4312,27.1352"
        };

        var izmir3 = new Restaurant
        {
            Name = "Kordon Kahvaltı",
            City = "İzmir",
            Address = "Alsancak, 1. Kordon No:66",
            Latitude = 38.4378,
            Longitude = 27.1421,
            LocationUrl = "https://maps.google.com/?q=38.4378,27.1421"
        };

        var izmir4 = new Restaurant
        {
            Name = "Ayşa Kebap",
            City = "İzmir",
            Address = "Bornova, Erzene Mah. No:34",
            Latitude = 38.4567,
            Longitude = 27.2134,
            LocationUrl = "https://maps.google.com/?q=38.4567,27.2134"
        };

        var izmir5 = new Restaurant
        {
            Name = "Reyhan Vejetaryen",
            City = "İzmir",
            Address = "Alsancak, Kıbrıs Şehitleri Cad. No:45",
            Latitude = 38.4345,
            Longitude = 27.1456,
            LocationUrl = "https://maps.google.com/?q=38.4345,27.1456"
        };

        var izmir6 = new Restaurant
        {
            Name = "Petra Roastery",
            City = "İzmir",
            Address = "Alsancak, 1453 Sok. No:12",
            Latitude = 38.4367,
            Longitude = 27.1412,
            LocationUrl = "https://maps.google.com/?q=38.4367,27.1412"
        };

        // =============================================
        // ANTALYA RESTAURANTS (6 adet)
        // =============================================
        var antalya1 = new Restaurant
        {
            Name = "Club Arma",
            City = "Antalya",
            Address = "Kaleiçi, Yat Limanı, Selçuk Mah.",
            Latitude = 36.8841,
            Longitude = 30.7056,
            LocationUrl = "https://maps.google.com/?q=36.8841,30.7056"
        };

        var antalya2 = new Restaurant
        {
            Name = "Seraser Fine Dining",
            City = "Antalya",
            Address = "Kaleiçi, Tuzcular Mah. Karanlık Sok. No:18",
            Latitude = 36.8856,
            Longitude = 30.7067,
            LocationUrl = "https://maps.google.com/?q=36.8856,30.7067"
        };

        var antalya3 = new Restaurant
        {
            Name = "Vanilla Lounge",
            City = "Antalya",
            Address = "Kaleiçi, Hesapçı Sok. No:33",
            Latitude = 36.8863,
            Longitude = 30.7078,
            LocationUrl = "https://maps.google.com/?q=36.8863,30.7078"
        };

        var antalya4 = new Restaurant
        {
            Name = "7 Mehmet Restaurant",
            City = "Antalya",
            Address = "Konyaaltı, Liman Cad. No:7",
            Latitude = 36.8512,
            Longitude = 30.6234,
            LocationUrl = "https://maps.google.com/?q=36.8512,30.6234"
        };

        var antalya5 = new Restaurant
        {
            Name = "Pio Gastro Bar",
            City = "Antalya",
            Address = "Lara, Güzeloba Mah. Barınaklar Bulvarı",
            Latitude = 36.8612,
            Longitude = 30.7834,
            LocationUrl = "https://maps.google.com/?q=36.8612,30.7834"
        };

        var antalya6 = new Restaurant
        {
            Name = "Köfteci Yusuf",
            City = "Antalya",
            Address = "Muratpaşa, Şarampol Cad. No:15",
            Latitude = 36.8821,
            Longitude = 30.6987,
            LocationUrl = "https://maps.google.com/?q=36.8821,30.6987"
        };

        // Add all restaurants
        var allRestaurants = new List<Restaurant>
        {
            istanbul1, istanbul2, istanbul3, istanbul4, istanbul5, istanbul6, istanbul7, istanbul8,
            ankara1, ankara2, ankara3, ankara4, ankara5, ankara6,
            izmir1, izmir2, izmir3, izmir4, izmir5, izmir6,
            antalya1, antalya2, antalya3, antalya4, antalya5, antalya6
        };

        context.Restaurants.AddRange(allRestaurants);
        context.SaveChanges();

        // =============================================
        // FOOD ITEMS - Her restoran için 3-4 yemek
        // =============================================
        var foodItems = new List<FoodItem>();

        // --- Istanbul 1: Nusr-Et Steakhouse ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul1.Id,
            Name = "Saltbae Özel Bonfile",
            ImageUrl = "https://images.unsplash.com/photo-1546964124-0cce460f38ef?w=600",
            Tags = "FineDining,Luxury,Et",
            MealType = "Dinner"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul1.Id,
            Name = "Kuzu Pirzola",
            ImageUrl = "https://images.unsplash.com/photo-1432139555190-58524dae6a55?w=600",
            Tags = "FineDining,Et,Luxury",
            MealType = "Dinner"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul1.Id,
            Name = "Lokum Biftek",
            ImageUrl = "https://images.unsplash.com/photo-1588168333986-5078d3ae3976?w=600",
            Tags = "FineDining,Et,Premium",
            MealType = "Dinner"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul1.Id,
            Name = "Burger Steak",
            ImageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=600",
            Tags = "Burger,Et,Casual",
            MealType = "Lunch"
        });

        // --- Istanbul 2: Mikla Restaurant ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul2.Id,
            Name = "Anadolu Tadım Menüsü",
            ImageUrl = "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=600",
            Tags = "FineDining,Luxury,Vegetarian",
            MealType = "Dinner"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul2.Id,
            Name = "Ege Otu Salatası",
            ImageUrl = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=600",
            Tags = "Vegan,Vegetarian,Healthy,Glutensiz",
            MealType = "Lunch"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul2.Id,
            Name = "Kuzu İncik",
            ImageUrl = "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=600",
            Tags = "FineDining,Et,Traditional",
            MealType = "Dinner"
        });

        // --- Istanbul 3: Van Kahvaltı Evi ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul3.Id,
            Name = "Serpme Van Kahvaltısı",
            ImageUrl = "https://images.unsplash.com/photo-1533089860892-a7c6f0a88666?w=600",
            Tags = "Breakfast,Traditional,Vegetarian",
            MealType = "Breakfast"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul3.Id,
            Name = "Van Otlu Peynir Tabağı",
            ImageUrl = "https://images.unsplash.com/photo-1552767059-ce182ead6c1b?w=600",
            Tags = "Breakfast,Vegetarian,Traditional",
            MealType = "Breakfast"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul3.Id,
            Name = "Kaymak ve Bal",
            ImageUrl = "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=600",
            Tags = "Breakfast,Dessert,Traditional",
            MealType = "Breakfast"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul3.Id,
            Name = "Kavut",
            ImageUrl = "https://images.unsplash.com/photo-1525351484163-7529414344d8?w=600",
            Tags = "Breakfast,Traditional,Sweet",
            MealType = "Breakfast"
        });

        // --- Istanbul 4: Karaköy Güllüoğlu ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul4.Id,
            Name = "Fıstıklı Baklava",
            ImageUrl = "https://images.unsplash.com/photo-1519676867240-f03562e64548?w=600",
            Tags = "Dessert,Traditional,Sweet",
            MealType = "Dessert"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul4.Id,
            Name = "Şöbiyet",
            ImageUrl = "https://images.unsplash.com/photo-1598110750624-207050c4f28c?w=600",
            Tags = "Dessert,Traditional,Creamy",
            MealType = "Dessert"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul4.Id,
            Name = "Havuç Dilimi",
            ImageUrl = "https://images.unsplash.com/photo-1571506165871-ee72a35bc9d4?w=600",
            Tags = "Dessert,Traditional,Sweet",
            MealType = "Dessert"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul4.Id,
            Name = "Cevizli Baklava",
            ImageUrl = "https://images.unsplash.com/photo-1625535163131-9d1fc535b324?w=600",
            Tags = "Dessert,Traditional,Nutty",
            MealType = "Dessert"
        });

        // --- Istanbul 5: Çiya Sofrası ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul5.Id,
            Name = "Güneydoğu Meze Tabağı",
            ImageUrl = "https://images.unsplash.com/photo-1529006557810-274b9b2fc783?w=600",
            Tags = "Traditional,Vegetarian,StreetFood",
            MealType = "Lunch"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul5.Id,
            Name = "İçli Köfte",
            ImageUrl = "https://images.unsplash.com/photo-1529042410759-befb1204b468?w=600",
            Tags = "Traditional,Acılı,StreetFood",
            MealType = "Lunch"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul5.Id,
            Name = "Kuru Dolma",
            ImageUrl = "https://images.unsplash.com/photo-1541014741259-de529411b96a?w=600",
            Tags = "Traditional,Vegetarian",
            MealType = "Lunch"
        });

        // --- Istanbul 6: Kronotrop Coffee ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul6.Id,
            Name = "V60 Pour Over",
            ImageUrl = "https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?w=600",
            Tags = "Coffee,Modern,Premium",
            MealType = "Coffee"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul6.Id,
            Name = "Flat White",
            ImageUrl = "https://images.unsplash.com/photo-1461023058943-07fcbe16d735?w=600",
            Tags = "Coffee,Modern,Creamy",
            MealType = "Coffee"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul6.Id,
            Name = "Chemex Cold Brew",
            ImageUrl = "https://images.unsplash.com/photo-1517701604599-bb29b565090c?w=600",
            Tags = "Coffee,Cold,Modern",
            MealType = "Coffee"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul6.Id,
            Name = "Avokadolu Tost",
            ImageUrl = "https://images.unsplash.com/photo-1541519227354-08fa5d50c44d?w=600",
            Tags = "Breakfast,Vegan,Healthy",
            MealType = "Breakfast"
        });

        // --- Istanbul 7: Kanaat Lokantası ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul7.Id,
            Name = "Hünkar Beğendi",
            ImageUrl = "https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=600",
            Tags = "Traditional,Et,Comfort",
            MealType = "Dinner"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul7.Id,
            Name = "Fırın Sütlaç",
            ImageUrl = "https://images.unsplash.com/photo-1571877227200-a0d98ea607e9?w=600",
            Tags = "Dessert,Traditional,Creamy",
            MealType = "Dessert"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul7.Id,
            Name = "Patlıcan Musakka",
            ImageUrl = "https://images.unsplash.com/photo-1544025162-d76694e18b17?w=600",
            Tags = "Traditional,Vegetarian,Comfort",
            MealType = "Lunch"
        });

        // --- Istanbul 8: Baylan Pastanesi ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul8.Id,
            Name = "Profiterol",
            ImageUrl = "https://images.unsplash.com/photo-1587314168485-3236d6710814?w=600",
            Tags = "Dessert,Chocolate,Sweet",
            MealType = "Dessert"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul8.Id,
            Name = "Kup Griye",
            ImageUrl = "https://images.unsplash.com/photo-1551024601-bec78aea704b?w=600",
            Tags = "Dessert,IceCream,Classic",
            MealType = "Dessert"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = istanbul8.Id,
            Name = "Trileçe",
            ImageUrl = "https://images.unsplash.com/photo-1578985545062-69928b1d9587?w=600",
            Tags = "Dessert,Cake,Creamy",
            MealType = "Dessert"
        });

        // --- Ankara 1: Trilye Restaurant ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = ankara1.Id,
            Name = "Ege Deniz Ürünleri",
            ImageUrl = "https://images.unsplash.com/photo-1559339352-11d035aa65de?w=600",
            Tags = "Seafood,FineDining,Luxury",
            MealType = "Dinner"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = ankara1.Id,
            Name = "Levrek Marine",
            ImageUrl = "https://images.unsplash.com/photo-1519708227418-c8fd9a32b7a2?w=600",
            Tags = "Seafood,FineDining,Fresh",
            MealType = "Dinner"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = ankara1.Id,
            Name = "Ahtapot Izgara",
            ImageUrl = "https://images.unsplash.com/photo-1565680018434-b513d5e5fd47?w=600",
            Tags = "Seafood,Grilled,Premium",
            MealType = "Dinner"
        });

        // --- Ankara 2: Hacı Arif Bey Kebapçısı ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = ankara2.Id,
            Name = "Adana Kebap",
            ImageUrl = "https://images.unsplash.com/photo-1599487488170-d11ec9c172f0?w=600",
            Tags = "Kebap,Acılı,Traditional",
            MealType = "Dinner"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = ankara2.Id,
            Name = "Urfa Kebap",
            ImageUrl = "https://images.unsplash.com/photo-1529042410759-befb1204b468?w=600",
            Tags = "Kebap,Traditional,Mild",
            MealType = "Dinner"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = ankara2.Id,
            Name = "Patlıcan Kebabı",
            ImageUrl = "https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=600",
            Tags = "Kebap,Traditional,Et",
            MealType = "Dinner"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = ankara2.Id,
            Name = "Lahmacun",
            ImageUrl = "https://images.unsplash.com/photo-1628840042765-356cda07504e?w=600",
            Tags = "StreetFood,Acılı,Traditional",
            MealType = "Lunch"
        });

        // --- Ankara 3: Sedef Serpme Kahvaltı ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = ankara3.Id,
            Name = "Serpme Kahvaltı",
            ImageUrl = "https://images.unsplash.com/photo-1525351484163-7529414344d8?w=600",
            Tags = "Breakfast,Traditional,Vegetarian",
            MealType = "Breakfast"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = ankara3.Id,
            Name = "Menemen",
            ImageUrl = "https://images.unsplash.com/photo-1590412200988-a436970781fa?w=600",
            Tags = "Breakfast,Vegetarian,Hot",
            MealType = "Breakfast"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = ankara3.Id,
            Name = "Sucuklu Yumurta",
            ImageUrl = "https://images.unsplash.com/photo-1476718406336-bb5a9690ee2a?w=600",
            Tags = "Breakfast,Et,Traditional",
            MealType = "Breakfast"
        });

        // --- Ankara 4: Günaydın Kasap & Steakhouse ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = ankara4.Id,
            Name = "Dana Bonfile",
            ImageUrl = "https://images.unsplash.com/photo-1558030006-450675393462?w=600",
            Tags = "Steak,Premium,Et",
            MealType = "Dinner"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = ankara4.Id,
            Name = "Antrikot",
            ImageUrl = "https://images.unsplash.com/photo-1546964124-0cce460f38ef?w=600",
            Tags = "Steak,Grilled,Premium",
            MealType = "Dinner"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = ankara4.Id,
            Name = "Burger Plate",
            ImageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=600",
            Tags = "Burger,StreetFood,Casual",
            MealType = "Lunch"
        });

        // --- Ankara 5: D'oreille Patisserie ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = ankara5.Id,
            Name = "Macaron Çeşitleri",
            ImageUrl = "https://images.unsplash.com/photo-1569864358642-9d1684040f43?w=600",
            Tags = "Dessert,French,Sweet",
            MealType = "Dessert"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = ankara5.Id,
            Name = "Croissant",
            ImageUrl = "https://images.unsplash.com/photo-1555507036-ab1f4038808a?w=600",
            Tags = "Breakfast,French,Pastry",
            MealType = "Breakfast"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = ankara5.Id,
            Name = "Mille Feuille",
            ImageUrl = "https://images.unsplash.com/photo-1612203985729-70726954388c?w=600",
            Tags = "Dessert,French,Creamy",
            MealType = "Dessert"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = ankara5.Id,
            Name = "Ekler",
            ImageUrl = "https://images.unsplash.com/photo-1528975604071-b4dc52a2d18c?w=600",
            Tags = "Dessert,Chocolate,French",
            MealType = "Dessert"
        });

        // --- Ankara 6: Vegan Mutfak ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = ankara6.Id,
            Name = "Buddha Bowl",
            ImageUrl = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=600",
            Tags = "Vegan,Healthy,Glutensiz",
            MealType = "Lunch"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = ankara6.Id,
            Name = "Vegan Burger",
            ImageUrl = "https://images.unsplash.com/photo-1585238341710-4d3ff484184d?w=600",
            Tags = "Vegan,Burger,StreetFood",
            MealType = "Lunch"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = ankara6.Id,
            Name = "Smoothie Bowl",
            ImageUrl = "https://images.unsplash.com/photo-1590301157890-4810ed352733?w=600",
            Tags = "Vegan,Breakfast,Healthy",
            MealType = "Breakfast"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = ankara6.Id,
            Name = "Raw Cheesecake",
            ImageUrl = "https://images.unsplash.com/photo-1565958011703-44f9829ba187?w=600",
            Tags = "Vegan,Dessert,Glutensiz",
            MealType = "Dessert"
        });

        // --- Izmir 1: Sakız Restaurant ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = izmir1.Id,
            Name = "Ege Mezeler Tabağı",
            ImageUrl = "https://images.unsplash.com/photo-1540189549336-e6e99c3679fe?w=600",
            Tags = "Traditional,Vegetarian,Local",
            MealType = "Lunch"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = izmir1.Id,
            Name = "Zeytinyağlı Enginar",
            ImageUrl = "https://images.unsplash.com/photo-1574484284002-952d92456975?w=600",
            Tags = "Vegan,Traditional,Healthy",
            MealType = "Lunch"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = izmir1.Id,
            Name = "Ege Çırpması",
            ImageUrl = "https://images.unsplash.com/photo-1511690656952-34342bb7c2f2?w=600",
            Tags = "Seafood,Local,Traditional",
            MealType = "Dinner"
        });

        // --- Izmir 2: Deniz Restaurant ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = izmir2.Id,
            Name = "Çupra Izgara",
            ImageUrl = "https://images.unsplash.com/photo-1519708227418-c8fd9a32b7a2?w=600",
            Tags = "Seafood,Grilled,Fresh",
            MealType = "Dinner"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = izmir2.Id,
            Name = "Karides Güveç",
            ImageUrl = "https://images.unsplash.com/photo-1565680018434-b513d5e5fd47?w=600",
            Tags = "Seafood,Hot,Traditional",
            MealType = "Dinner"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = izmir2.Id,
            Name = "Midye Tava",
            ImageUrl = "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=600",
            Tags = "Seafood,StreetFood,Fried",
            MealType = "Lunch"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = izmir2.Id,
            Name = "Kalamar Tava",
            ImageUrl = "https://images.unsplash.com/photo-1599487488170-d11ec9c172f0?w=600",
            Tags = "Seafood,StreetFood,Crispy",
            MealType = "Lunch"
        });

        // --- Izmir 3: Kordon Kahvaltı ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = izmir3.Id,
            Name = "İzmir Serpme Kahvaltı",
            ImageUrl = "https://images.unsplash.com/photo-1550304943-4f24f54ddde9?w=600",
            Tags = "Breakfast,Traditional,Local",
            MealType = "Breakfast"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = izmir3.Id,
            Name = "Boyoz ve Gevrek",
            ImageUrl = "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=600",
            Tags = "Breakfast,StreetFood,Local",
            MealType = "Breakfast"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = izmir3.Id,
            Name = "Tulum Peynirli Omlet",
            ImageUrl = "https://images.unsplash.com/photo-1525351484163-7529414344d8?w=600",
            Tags = "Breakfast,Vegetarian,Local",
            MealType = "Breakfast"
        });

        // --- Izmir 4: Ayşa Kebap ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = izmir4.Id,
            Name = "İzmir Köfte",
            ImageUrl = "https://images.unsplash.com/photo-1529042410759-befb1204b468?w=600",
            Tags = "Kebap,Traditional,Et",
            MealType = "Lunch"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = izmir4.Id,
            Name = "Şiş Kebap",
            ImageUrl = "https://images.unsplash.com/photo-1599487488170-d11ec9c172f0?w=600",
            Tags = "Kebap,Grilled,Traditional",
            MealType = "Dinner"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = izmir4.Id,
            Name = "Kuzu Tandır",
            ImageUrl = "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=600",
            Tags = "Traditional,SlowCook,Premium",
            MealType = "Dinner"
        });

        // --- Izmir 5: Reyhan Vejetaryen ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = izmir5.Id,
            Name = "Ot Kavurma",
            ImageUrl = "https://images.unsplash.com/photo-1504754524776-8f4f37790ca0?w=600",
            Tags = "Vegan,Traditional,Healthy",
            MealType = "Lunch"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = izmir5.Id,
            Name = "Zeytinyağlılar Tabağı",
            ImageUrl = "https://images.unsplash.com/photo-1540189549336-e6e99c3679fe?w=600",
            Tags = "Vegan,Traditional,Cold",
            MealType = "Lunch"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = izmir5.Id,
            Name = "Kabak Mücver",
            ImageUrl = "https://images.unsplash.com/photo-1546549032-9571cd6b27df?w=600",
            Tags = "Vegetarian,StreetFood,Traditional",
            MealType = "Lunch"
        });

        // --- Izmir 6: Petra Roastery ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = izmir6.Id,
            Name = "Specialty Latte",
            ImageUrl = "https://images.unsplash.com/photo-1461023058943-07fcbe16d735?w=600",
            Tags = "Coffee,Modern,Creamy",
            MealType = "Coffee"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = izmir6.Id,
            Name = "Cold Brew Tonic",
            ImageUrl = "https://images.unsplash.com/photo-1517701604599-bb29b565090c?w=600",
            Tags = "Coffee,Cold,Modern",
            MealType = "Coffee"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = izmir6.Id,
            Name = "Affogato",
            ImageUrl = "https://images.unsplash.com/photo-1579992357154-faf4bde95b3d?w=600",
            Tags = "Coffee,Dessert,IceCream",
            MealType = "Dessert"
        });

        // --- Antalya 1: Club Arma ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = antalya1.Id,
            Name = "Akdeniz Meze Tabağı",
            ImageUrl = "https://images.unsplash.com/photo-1559339352-11d035aa65de?w=600",
            Tags = "Seafood,FineDining,Mediterranean",
            MealType = "Dinner"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = antalya1.Id,
            Name = "Istakoz Thermidor",
            ImageUrl = "https://images.unsplash.com/photo-1565680018434-b513d5e5fd47?w=600",
            Tags = "Seafood,Luxury,Premium",
            MealType = "Dinner"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = antalya1.Id,
            Name = "Karışık Izgara Balık",
            ImageUrl = "https://images.unsplash.com/photo-1580476262798-bddd9f4b7369?w=600",
            Tags = "Seafood,Grilled,Fresh",
            MealType = "Dinner"
        });

        // --- Antalya 2: Seraser Fine Dining ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = antalya2.Id,
            Name = "Ottoman Lezzetleri",
            ImageUrl = "https://images.unsplash.com/photo-1611143669185-af224c5e3252?w=600",
            Tags = "FineDining,Luxury,Traditional",
            MealType = "Dinner"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = antalya2.Id,
            Name = "Kuzu Tandir",
            ImageUrl = "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=600",
            Tags = "FineDining,Et,SlowCook",
            MealType = "Dinner"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = antalya2.Id,
            Name = "Türk Kahveli Panna Cotta",
            ImageUrl = "https://images.unsplash.com/photo-1488477181946-6428a0291777?w=600",
            Tags = "Dessert,FineDining,Modern",
            MealType = "Dessert"
        });

        // --- Antalya 3: Vanilla Lounge ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = antalya3.Id,
            Name = "Akdeniz Serpme Kahvaltı",
            ImageUrl = "https://images.unsplash.com/photo-1504754524776-8f4f37790ca0?w=600",
            Tags = "Breakfast,Traditional,Healthy",
            MealType = "Breakfast"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = antalya3.Id,
            Name = "Acai Bowl",
            ImageUrl = "https://images.unsplash.com/photo-1590301157890-4810ed352733?w=600",
            Tags = "Breakfast,Vegan,Healthy",
            MealType = "Breakfast"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = antalya3.Id,
            Name = "Waffle Plate",
            ImageUrl = "https://images.unsplash.com/photo-1562376552-0d160a2f238d?w=600",
            Tags = "Breakfast,Sweet,Modern",
            MealType = "Breakfast"
        });

        // --- Antalya 4: 7 Mehmet Restaurant ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = antalya4.Id,
            Name = "Tandir Kuzu",
            ImageUrl = "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=600",
            Tags = "Traditional,Et,SlowCook",
            MealType = "Dinner"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = antalya4.Id,
            Name = "Şiş Tavuk",
            ImageUrl = "https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=600",
            Tags = "Grilled,Traditional,Healthy",
            MealType = "Dinner"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = antalya4.Id,
            Name = "Pide Çeşitleri",
            ImageUrl = "https://images.unsplash.com/photo-1628840042765-356cda07504e?w=600",
            Tags = "Traditional,StreetFood,Cheese",
            MealType = "Lunch"
        });

        // --- Antalya 5: Pio Gastro Bar ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = antalya5.Id,
            Name = "Vegan Buddha Bowl",
            ImageUrl = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=600",
            Tags = "Vegan,Healthy,Modern",
            MealType = "Lunch"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = antalya5.Id,
            Name = "Truffle Risotto",
            ImageUrl = "https://images.unsplash.com/photo-1476124369491-e7addf5db371?w=600",
            Tags = "Vegetarian,FineDining,Italian",
            MealType = "Dinner"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = antalya5.Id,
            Name = "Craft Cocktails",
            ImageUrl = "https://images.unsplash.com/photo-1514362545857-3bc16c4c7d1b?w=600",
            Tags = "Drinks,Modern,Bar",
            MealType = "Drinks"
        });

        // --- Antalya 6: Köfteci Yusuf ---
        foodItems.Add(new FoodItem
        {
            RestaurantId = antalya6.Id,
            Name = "Izgara Köfte",
            ImageUrl = "https://images.unsplash.com/photo-1529042410759-befb1204b468?w=600",
            Tags = "StreetFood,Traditional,Et",
            MealType = "Lunch"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = antalya6.Id,
            Name = "Kasap Köfte",
            ImageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=600",
            Tags = "StreetFood,Traditional,Grilled",
            MealType = "Lunch"
        });
        foodItems.Add(new FoodItem
        {
            RestaurantId = antalya6.Id,
            Name = "Piyaz",
            ImageUrl = "https://images.unsplash.com/photo-1540189549336-e6e99c3679fe?w=600",
            Tags = "Vegan,Traditional,Cold",
            MealType = "Lunch"
        });

        context.FoodItems.AddRange(foodItems);
        context.SaveChanges();
    }
}
