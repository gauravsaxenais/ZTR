# This thin wrapper Makefile is used to pull in the spork submodule if
# it doesn't yet exist (i.e you didn't perform a recursive clone or call git
# submodule update --init). The target is configured such that it will only
# execute if the target build framework Makefile doesn't yet exists. This
# wrapper allows you to call make [TARGET] and transparently pull down the
# framework in the background if needed before continuing.

-include $(CURDIR)/spork/inc.mk

$(CURDIR)/spork/inc.mk:
	@echo -e "\033[38;5;208m""warn""\033[0m"": Couldn't find spork, updating now ..."
	@git submodule update --init $(CURDIR)/spork
