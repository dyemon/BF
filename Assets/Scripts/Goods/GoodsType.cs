public enum GoodsTypeGroup {
	IncreeseDamage = 0, IncreeseHealth = 100
}

public enum GoodsType {
	None = -1,

	Espander = GoodsTypeGroup.IncreeseDamage,
	Gruzilo = GoodsTypeGroup.IncreeseDamage + 1,
	Kluchi = GoodsTypeGroup.IncreeseDamage + 2,
	Rozochka = GoodsTypeGroup.IncreeseDamage + 3,
	Cep = GoodsTypeGroup.IncreeseDamage + 4,
	Dereviashka = GoodsTypeGroup.IncreeseDamage + 5,
	Truba = GoodsTypeGroup.IncreeseDamage + 6,
	Molotok = GoodsTypeGroup.IncreeseDamage + 7,
	GaechniyKluch = GoodsTypeGroup.IncreeseDamage + 8,
	Gvozdoder = GoodsTypeGroup.IncreeseDamage + 9,
	Kastet = GoodsTypeGroup.IncreeseDamage + 10,
	Bita = GoodsTypeGroup.IncreeseDamage + 11,
	Dubinka = GoodsTypeGroup.IncreeseDamage + 12,
	
	Zazhigalka = GoodsTypeGroup.IncreeseHealth,
	Karti = GoodsTypeGroup.IncreeseHealth + 1,
	Znachok = GoodsTypeGroup.IncreeseHealth + 2,
	Baclazhka = GoodsTypeGroup.IncreeseHealth + 3,
	Stakan = GoodsTypeGroup.IncreeseHealth + 4,
	Krosovki = GoodsTypeGroup.IncreeseHealth + 5,
	Noski = GoodsTypeGroup.IncreeseHealth + 6,
	Telogreyka = GoodsTypeGroup.IncreeseHealth + 7,
	Shapka = GoodsTypeGroup.IncreeseHealth + 8,
	Botinki = GoodsTypeGroup.IncreeseHealth + 9,
	Barsetka = GoodsTypeGroup.IncreeseHealth + 10,
	Chotki = GoodsTypeGroup.IncreeseHealth + 11,
	Chasi = GoodsTypeGroup.IncreeseHealth + 12,
	Mashina = GoodsTypeGroup.IncreeseHealth + 13
}