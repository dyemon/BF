public enum GoodTypeGroup {
	IncreeseDamage = 0, IncreeseHealth = 100
}

public enum GoodType {
	Espander = GoodTypeGroup.IncreeseDamage,
	Gruzilo = GoodTypeGroup.IncreeseDamage + 1,
	Kluchi = GoodTypeGroup.IncreeseDamage + 2,
	Rozochka = GoodTypeGroup.IncreeseDamage + 3,
	Cep = GoodTypeGroup.IncreeseDamage + 4,
	Dereviashka = GoodTypeGroup.IncreeseDamage + 5,
	Truba = GoodTypeGroup.IncreeseDamage + 6,
	Molotok = GoodTypeGroup.IncreeseDamage + 7,
	GaechniyKluch = GoodTypeGroup.IncreeseDamage + 8,
	Gvozdoder = GoodTypeGroup.IncreeseDamage + 9,
	Kastet = GoodTypeGroup.IncreeseDamage + 10,
	Bita = GoodTypeGroup.IncreeseDamage + 11,
	Dubinka = GoodTypeGroup.IncreeseDamage + 12,
	
	Zazhigalka = GoodTypeGroup.IncreeseHealth,
	Karti = GoodTypeGroup.IncreeseHealth + 1,
	Znachok = GoodTypeGroup.IncreeseHealth + 2,
	Baclazhka = GoodTypeGroup.IncreeseHealth + 3,
	Stakan = GoodTypeGroup.IncreeseHealth + 4,
	Krosovki = GoodTypeGroup.IncreeseHealth + 5,
	Noski = GoodTypeGroup.IncreeseHealth + 6,
	Telogreyka = GoodTypeGroup.IncreeseHealth + 7,
	Shapka = GoodTypeGroup.IncreeseHealth + 8,
	Botinki = GoodTypeGroup.IncreeseHealth + 9,
	Barsetka = GoodTypeGroup.IncreeseHealth + 10,
	Chotki = GoodTypeGroup.IncreeseHealth + 11,
	Chasi = GoodTypeGroup.IncreeseHealth + 12,
	BarsetkaPokruchi = GoodTypeGroup.IncreeseHealth + 13,
	Mashina = GoodTypeGroup.IncreeseHealth + 14
}