class_name AttributesAndAbilities
extends BaseStats

signal rolled
signal succeeded
signal one

# attributes
@export var charisma := 1
@export var manipulation := 1
@export var appearance := 0
@export var perception := 1
@export var intelligence := 1
@export var wits := 1
# abilities
@export var awareness := 0
@export var brawl := 0
@export var empathy := 0
@export var intimidation := 0
@export var subterfuge := 0

@export var firearms := 0
@export var larceny := 0
@export var etiquette := 0
@export var crafts := 0
@export var stealth := 0

@export var technology := 0
@export var investigation := 0
@export var academics := 0
@export var occult := 0
@export var politics := 0

@export var max_attribute := 5

@export var stat_list := {
	"strength" = strength,
	"dexterity" = dexterity,
	"stamina" = stamina,
	"awareness" = awareness,
	"brawl" = brawl,
	"empathy" = empathy,
	"intimidation" = intimidation,
	"subterfuge" = subterfuge,
	"charisma" = charisma,
	"manipulation" = manipulation,
	"appearance" = appearance,
	"firearms" = firearms,
	"larceny" = larceny,
	"etiquette" = etiquette,
	"crafts" = crafts,
	"stealth" = stealth,
	"perception" = perception,
	"intelligence" = intelligence,
	"wits" = wits,
	"technology" = technology,
	"investigation" = investigation,
	"academics" = academics,
	"occult" = occult,
	"politics" = politics,
	"humanity" = humanity,
	"willpower" = willpower,
}

#I'll fix this later
#@export var big_stats_list := {
	#"humanity" = humanity,
	#"willpower" = willpower,
	# "blood_pool" = blood_pool, 
#}

func stats_inc_dec(value : int, stat : String) -> void:
	if value == 0 or self.get(stat) == null:
		return
	else:
		self.set(stat, clampi(self.get(stat) + value, 0, max_attribute))
		emit_signal("stats_changed")

func roll(attribute : String, ability : String, difficulty : int) -> int:
	var success := 0
	if self.get(attribute) == self.get(ability):
		return 0
	emit_signal("rolled")
	for n in (self.get(attribute) + self.get(ability)):
		var result := randi_range(1, 10)
		if result >= difficulty:
			success += 1
			emit_signal("succeeded")
		elif result == 1:
			success -= 1
			emit_signal("one")
	return success
