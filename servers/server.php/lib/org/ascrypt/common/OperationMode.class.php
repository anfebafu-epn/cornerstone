<?php

class org_ascrypt_common_OperationMode {
	public function __construct(){}
	static $ECB = "ecb";
	static $CBC = "cbc";
	static $CTR = "ctr";
	static $NONE = "none";
	function __toString() { return 'org.ascrypt.common.OperationMode'; }
}
