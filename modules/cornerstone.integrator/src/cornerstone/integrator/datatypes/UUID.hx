package cornerstone.integrator.datatypes;
import haxe.io.Bytes;

@:keep
@:keepSub
class UUID {

// It stores UUID in a String.
    private var UUIDValue:String = "00000000-0000-0000-0000-000000000000";

    public static var EMPTY:UUID = new UUID();
    private static var CHARS = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".split("");
    private inline static var MPM = 2147483647.0;
    private inline static var MINSTD = 16807.0;

    public function new() {
    }

    public function set_Value(UUIDString:String):UUID {
        this.UUIDValue = UUIDString;
        return this;
    }

/**
	* A more performant, but slightly bulkier, RFC4122v4 solution.  We boost performance
	* by minimizing calls to random()
	*/

    public static function newUUID():UUID {
        var seed:Int = makeRandomSeed();
        var chars = CHARS, uuid = new Array(), rnd = 0, r;
        for (i in 0...36) {
            if (i == 8 || i == 13 || i == 18 || i == 23) {
                uuid[i] = "-";
            } else if (i == 14) {
                uuid[i] = "4";
            } else {
                if (rnd <= 0x02) rnd = 0x2000000 + Std.int((cast((seed = nextParkMiller(seed)), Float) * 0x1000000)) | 0;
//if (rnd <= 0x02) rnd = 0x2000000 + ((seed = nextParkMiller(seed)).toFloat() * 0x1000000).int() | 0;
                r = rnd & 0xf;
                rnd = rnd >> 4;
                uuid[i] = chars[(i == 19) ? (r & 0x3) | 0x8 : r];
            }
        }
        return new UUID().set_Value(uuid.join(""));
    }


/**
	 * Make a non deterministic random seed using standard libraries.
	 * @return Non deterministic random seed.
	 */

    public static function makeRandomSeed():Int {
        return Math.floor(Math.random() * MPM);
    }

/**
	 * Park-Miller-Carta algorithm.
	 * @see <a href="http://lab.polygonal.de/?p=162">http://lab.polygonal.de/?p=162</a>
	 * @see <a href="http://code.google.com/p/polygonal/source/browse/trunk/src/lib/de/polygonal/core/math/random/ParkMiller.hx?r=547">http://code.google.com/p/polygonal/source/browse/trunk/src/lib/de/polygonal/core/math/random/ParkMiller.hx?r=547</a>
	 * @see <a href="http://en.wikipedia.org/wiki/Lehmer_random_number_generator">http://en.wikipedia.org/wiki/Lehmer_random_number_generator</a>
	 * @return Returns the next pseudo-random int value.
	 */

    public static function nextParkMiller(seed:Int):Int {

        return Std.int((seed * MINSTD) % MPM);
    }

    public function toBytes():Bytes {
        var GeneratedUUID:Bytes = Bytes.alloc(16);
        var SimpleUUID = StringTools.replace(UUIDValue, "-", "");
        var counter = 0;
        var lastvalue = 0;
        var bytespos = 0;

        for (i in 0...SimpleUUID.length) {
            var HexLetter = SimpleUUID.charAt(i);
            var value = 0;

            switch (HexLetter){
                case "1":
                    value = 1;
                case "2":
                    value = 2;
                case "3":
                    value = 3;
                case "4":
                    value = 4;
                case "5":
                    value = 5;
                case "6":
                    value = 6;
                case "7":
                    value = 7;
                case "8":
                    value = 8;
                case "9":
                    value = 9;
                case "A":
                    value = 10;
                case "B":
                    value = 11;
                case "C":
                    value = 12;
                case "D":
                    value = 13;
                case "E":
                    value = 14;
                case "F":
                    value = 15;
            }

            counter++;

            if (counter == 2) {
                counter = 0;
                GeneratedUUID.set(bytespos, lastvalue | value >> 4);
                bytespos++;
                lastvalue = 0;
            }
            else {
                lastvalue = value;
            }
        }

        return GeneratedUUID;
    }

    public static function FromBytes(UUIDBytes:Bytes):UUID {
        var ReadedUUID:String = "";
        var HexLetters = "0123456789ABCDEF";
        var LetterCounter = 0;

        for (i in 0...UUIDBytes.length) {
            var Readed:Int = UUIDBytes.get(i);

            var First:Int = Readed << 4;
            var Second:Int = Readed >> 4;

            ReadedUUID += HexLetters.charAt(First);
            ReadedUUID += HexLetters.charAt(Second);
            LetterCounter ++;

            if (LetterCounter == 9 || LetterCounter == 14 || LetterCounter == 19 || LetterCounter == 24) {
                ReadedUUID += "-";
                LetterCounter ++;
            }
        }
        return new UUID().set_Value(ReadedUUID);
    }
}
