# Changelog

## [0.2.2](https://github.com/albrtbc/gitstation/compare/gitstation-v0.2.1...gitstation-v0.2.2) (2026-03-29)


### Features

* show CI status on pull requests ([d5a8d42](https://github.com/albrtbc/gitstation/commit/d5a8d4253d76c18ccda831c93777c44a83c56494))


### Bug Fixes

* add breadcrumb bar when viewing a submodule repository ([2bc3383](https://github.com/albrtbc/gitstation/commit/2bc33837ab93a9f60d7cd174732d278c416ea0f1))
* dashboard repo list font size follows DefaultFontSize preference ([13c63c6](https://github.com/albrtbc/gitstation/commit/13c63c63cbaa30373fb3169eb3bc93a47fa077a4))
* open submodules in Dashboard instead of creating a new tab ([feec212](https://github.com/albrtbc/gitstation/commit/feec212f385812557602e3236b1b4b23d19709f4))
* prune stale remote branches on fetch ([cd8af7f](https://github.com/albrtbc/gitstation/commit/cd8af7f35f69f83c00d8c1da6524285d0d0f718f))
* replace anonymous types with concrete payloads for JSON serialization ([a5a7c41](https://github.com/albrtbc/gitstation/commit/a5a7c41fbb0dc6f2d85fbc0d892fbbdc61cc5fb0))
* show active repository name in title bar ([53cdd53](https://github.com/albrtbc/gitstation/commit/53cdd53ecdb27bbe1353cfb968ae1cd08d379b16))
* use JSON source generators for GitHub API serialization ([091546f](https://github.com/albrtbc/gitstation/commit/091546f4b281f58c30f6c01b7dbd366bdc74de2d))

## [0.2.1](https://github.com/albrtbc/gitstation/compare/gitstation-v0.2.0...gitstation-v0.2.1) (2026-03-29)


### Bug Fixes

* repo list updates instantly via FileSystemWatcher, not polling ([bb87553](https://github.com/albrtbc/gitstation/commit/bb8755307e43980e2bf28cc7b88c0322196df3dd))
* update stale sourcegit references in packaging script and slnx ([86bad9a](https://github.com/albrtbc/gitstation/commit/86bad9acdf8e614e26c23642d1f6ebdcb301c60a))

## [0.2.0](https://github.com/albrtbc/gitstation/compare/gitstation-v0.1.0...gitstation-v0.2.0) (2026-03-29)


### ⚠ BREAKING CHANGES

* Application renamed from SourceGit to GitStation. Data directory moves from SourceGit to GitStation.
* change `Show Tags as Tree` and `Show Submodules as Tree` setting from global to per-repo's UI state
* Users will lost their old UI states. But the shared settings remains.

### Features

* add `Checkout Branch` command palette ([#1937](https://github.com/albrtbc/gitstation/issues/1937)) ([e6de365](https://github.com/albrtbc/gitstation/commit/e6de365aee499c5b65bd42ddde4ca6fd6fbab28f))
* add `CustomAction` to command palette ([#2165](https://github.com/albrtbc/gitstation/issues/2165)) ([00183fb](https://github.com/albrtbc/gitstation/commit/00183fb6b7b5509bd1c213a8c7141bdec20b07cf))
* add `Ghostty` to Linux terminals ([#2138](https://github.com/albrtbc/gitstation/issues/2138)) ([df06720](https://github.com/albrtbc/gitstation/commit/df06720f44338d77d56d276cb442d7dfacf2bceb))
* add `Move to Workspace` context menu entry for repository tab in main tab bar ([#1968](https://github.com/albrtbc/gitstation/issues/1968)) ([e0ccdf4](https://github.com/albrtbc/gitstation/commit/e0ccdf4f2b578acba193de5e1468d700b7ffb469))
* add `on-behalf-of:` keyword auto-completion ([2fdff53](https://github.com/albrtbc/gitstation/commit/2fdff53b2f4d2a0aa42c110de864118c7babdd81))
* add `Open File` command palette to quick open repo's file with default editor ([52160e1](https://github.com/albrtbc/gitstation/commit/52160e1ff9392bb093d046d59ea465475785a347))
* add `Refresh` context menu entry to repo tab header ([360596c](https://github.com/albrtbc/gitstation/commit/360596c21796aad40bf7a5373d4ab8fe5dc0fe20))
* add `Refs: ` keyword ([bc4f06e](https://github.com/albrtbc/gitstation/commit/bc4f06eda9c8146e9c26bb14ff6b3dbc8b50f657))
* add `Repository Configure` to command palette ([242ab5b](https://github.com/albrtbc/gitstation/commit/242ab5b0189a04cd4ae7432841b0f5a284a22008))
* add `Reset File(s) to <revision>` context menu entry to selected change(s) in compare view ([#2079](https://github.com/albrtbc/gitstation/issues/2079)) ([f545eea](https://github.com/albrtbc/gitstation/commit/f545eeacd809b4d030fbe628ca381059da2915c8))
* add `Reset File(s) to <revision>` context menu entry to selected change(s) in revision compare view ([#2079](https://github.com/albrtbc/gitstation/issues/2079)) ([96a9e99](https://github.com/albrtbc/gitstation/commit/96a9e9962197129d22d6d0b36d3ec39e587d457c))
* add `Use fixed tab width in titlebar` option back ([#1910](https://github.com/albrtbc/gitstation/issues/1910)) ([101ad13](https://github.com/albrtbc/gitstation/commit/101ad13d633b44fffaa6396d7c03c7d247e88876))
* add a `STAGE SELECTED & COMMIT` button to `Confirm Empty Commit` dialog ([f102c9e](https://github.com/albrtbc/gitstation/commit/f102c9e7c54caf6dc1209225849f5f735bd18b84))
* add a minimap to merge conflict editor ([bc2d13a](https://github.com/albrtbc/gitstation/commit/bc2d13a3b8a9929b45955fb7b09344404c25ecec))
* add a new commit message trailer - `Milestone:` ([c677dd9](https://github.com/albrtbc/gitstation/commit/c677dd94290fdf7b89108221a2c01ac2efd9f06d))
* add a repository configuration - `Ask before auto-updating submodules` (default `false`) ([#2045](https://github.com/albrtbc/gitstation/issues/2045)) ([3c9a792](https://github.com/albrtbc/gitstation/commit/3c9a7920a35a7eb376822c8b992eb786a35ebea4))
* add a search(filter) keyword for commands in command palette ([#1937](https://github.com/albrtbc/gitstation/issues/1937)) ([30d5700](https://github.com/albrtbc/gitstation/commit/30d5700f8815f432dd33112738074052bcc60953))
* add context menu entry `Apply Changes` to only apply changes of selected file from stash (not reset mode) ([#2122](https://github.com/albrtbc/gitstation/issues/2122)) ([cf3a1c2](https://github.com/albrtbc/gitstation/commit/cf3a1c2c5359cd3e09285ce15c4bd6310695f232))
* add fixup head into parent ([#1969](https://github.com/albrtbc/gitstation/issues/1969)) ([e130d35](https://github.com/albrtbc/gitstation/commit/e130d35d331205e30faa18a517891f28958d958f))
* add hotkey `Alt/⌥+Up` to goto child of selected commit ([#2104](https://github.com/albrtbc/gitstation/issues/2104)) ([4c772ca](https://github.com/albrtbc/gitstation/commit/4c772cafa891d50bab92bd7994401e54b912d31a))
* add hotkey `Alt+Down/⌥+Down` to goto parent of selected commit ([#2104](https://github.com/albrtbc/gitstation/issues/2104)) ([ef14642](https://github.com/albrtbc/gitstation/commit/ef14642d4ea482f6787155ffc47c6421321d9ee8))
* add interactive rebase to branch context menus ([#2130](https://github.com/albrtbc/gitstation/issues/2130)) ([a2429a0](https://github.com/albrtbc/gitstation/commit/a2429a0f415e0ff814bdb49119281c90d651e64d))
* add log for `git cherry-pick/rebase/revert/merge --continue/--skip/--abort` ([#2096](https://github.com/albrtbc/gitstation/issues/2096)) ([5f0bd16](https://github.com/albrtbc/gitstation/commit/5f0bd16eac57a9bbf1e7841cbf97b49278c4b9b6))
* add more commands in command palette ([#1937](https://github.com/albrtbc/gitstation/issues/1937)) ([6241882](https://github.com/albrtbc/gitstation/commit/6241882b71f6f27b4ed1ca7444756cfcff59f6bb))
* adds required metadata files for flatpak publishing ([#2085](https://github.com/albrtbc/gitstation/issues/2085)) ([eac208c](https://github.com/albrtbc/gitstation/commit/eac208ca0cd3371337f051ca6a97fa34316d0dcd))
* allow to switch between pages switcher and repository command palette ([a94975b](https://github.com/albrtbc/gitstation/commit/a94975bb027c194e56c630b0fdc0ae6ae297dce1))
* allows to compare selected tag with current HEAD ([7043a22](https://github.com/albrtbc/gitstation/commit/7043a22de653773638837125bd7cbfc8aea1cdc7))
* allows to edit branch's description (`branch.<name>.description`) and show it in branch's tooltip ([#1920](https://github.com/albrtbc/gitstation/issues/1920)) ([4d48d1f](https://github.com/albrtbc/gitstation/commit/4d48d1f922d4b30c8802aaff2b18520c6e30f911))
* allows to toggle `-w` option (ignore whitespace changes) while blaming a file ([#1838](https://github.com/albrtbc/gitstation/issues/1838)) ([4d14c79](https://github.com/albrtbc/gitstation/commit/4d14c79a19d3e56102cfda45fb2968d5a43bd79c))
* auto-complete git commit message keywords, such as `Co-authored-by: ` ([a744a93](https://github.com/albrtbc/gitstation/commit/a744a93560952a4daa91ca8dc631b3c07131fd2e))
* auto-detect HTTPS website for `git@<host>:<repo>` formatted remote ([#1636](https://github.com/albrtbc/gitstation/issues/1636)) ([44fc98c](https://github.com/albrtbc/gitstation/commit/44fc98c070e1b40e66d3d783f6d93c17ede7c3cb))
* auto-remove history filter after deleting branch or tag ([#1904](https://github.com/albrtbc/gitstation/issues/1904)) ([4f11d8a](https://github.com/albrtbc/gitstation/commit/4f11d8a0778149c6aa2cc2ea15276a433526fa17))
* auto-scroll to first change hunk when diff file changed ([#1655](https://github.com/albrtbc/gitstation/issues/1655)) ([#1684](https://github.com/albrtbc/gitstation/issues/1684)) ([0d7d9c2](https://github.com/albrtbc/gitstation/commit/0d7d9c256018a6d14e0117f4e675e3598800051c))
* built-in merge conflict solver ([#2070](https://github.com/albrtbc/gitstation/issues/2070)) ([e5a5fad](https://github.com/albrtbc/gitstation/commit/e5a5fad432e435ce75dc51563ee1d363b00e6f68))
* complete rebrand SourceGit → GitStation ([f6174d6](https://github.com/albrtbc/gitstation/commit/f6174d6faf460c11fffb651eca4b4a92423363c2))
* disable `IsSnapToTickEnabled` for subject guide length slider ([#2123](https://github.com/albrtbc/gitstation/issues/2123)) ([47dae06](https://github.com/albrtbc/gitstation/commit/47dae0691140f31e6ef326f6ad6291af3366b3df))
* enable `core.untrackedCache=true` and `status.showUntrackedFiles=all` while querying local changes include untracked files ([#2016](https://github.com/albrtbc/gitstation/issues/2016)) ([8e765be](https://github.com/albrtbc/gitstation/commit/8e765bede704187efa405132b2f63a74cef9df58))
* keyword in commit subject can end with `-` character ([c1634ae](https://github.com/albrtbc/gitstation/commit/c1634ae457defe3cf148939cc4a00774ee9f5d1a))
* Linux AppImages support portable data dir ([#2062](https://github.com/albrtbc/gitstation/issues/2062)) ([4716124](https://github.com/albrtbc/gitstation/commit/4716124b9e4ae467121ca215ee5a9b5ac892089e))
* multi-solution management ([#2043](https://github.com/albrtbc/gitstation/issues/2043)) ([3dbab1c](https://github.com/albrtbc/gitstation/commit/3dbab1c7bf9f32982da0db51a5512dcb2a996835))
* rebrand SourceGit to GitStation, add release-please with semver ([423726a](https://github.com/albrtbc/gitstation/commit/423726a1d4ed67107cd8dc6a6681e827cefc592a))
* remember last navigated block after toggling `Show All Lines` ([#1952](https://github.com/albrtbc/gitstation/issues/1952)) ([646b080](https://github.com/albrtbc/gitstation/commit/646b080a384e2f6e8fe435fb7182075b7543b74c))
* remember the last selection of tag type in `Create Tag` popup ([#2128](https://github.com/albrtbc/gitstation/issues/2128)) ([b7d643e](https://github.com/albrtbc/gitstation/commit/b7d643ee37533e4917788f0bb1796e502d3094ac))
* respect `$BROWSER` env in linux ([#1672](https://github.com/albrtbc/gitstation/issues/1672)) ([f736f3b](https://github.com/albrtbc/gitstation/commit/f736f3bdc63898b2a1946d8ab2749651768a5c1d))
* show `MINE` and `THEIRS` revision in merge conflict editor ([1d80d00](https://github.com/albrtbc/gitstation/commit/1d80d001dd7599e80ab33b482d4d6497b0f4334b))
* show build date in `About` dialog ([a6a86cd](https://github.com/albrtbc/gitstation/commit/a6a86cd9c547e15ba3d10b1d577c9d6caa406c63))
* show current version and the publish date of new version ([#1930](https://github.com/albrtbc/gitstation/issues/1930)) ([432a9ca](https://github.com/albrtbc/gitstation/commit/432a9ca7ce9b6990adfb897c3d2cfb5d99014ece))
* show total changes in revision compare and branch compare ([4692df9](https://github.com/albrtbc/gitstation/commit/4692df985f52cb549396737d69cce62e3d402ff1))
* support `visualstudio.com`  urls for `Azure DevOps` pull requests ([#2087](https://github.com/albrtbc/gitstation/issues/2087)) ([1652b05](https://github.com/albrtbc/gitstation/commit/1652b0500b812b917a91bbb187722de245c81df0))
* support to open VSCode workspace ([#2119](https://github.com/albrtbc/gitstation/issues/2119)) ([45a3936](https://github.com/albrtbc/gitstation/commit/45a3936e46fdc95f589a069728f1235050298c54))
* supports `R16F` and `R32F` dds image ([5ef6072](https://github.com/albrtbc/gitstation/commit/5ef607244a676adbf5fcdaa8e305b180e0888a36))
* supports CLI command `--file-history <file_path>` to view given file history ([#1892](https://github.com/albrtbc/gitstation/issues/1892)) ([36aeb3f](https://github.com/albrtbc/gitstation/commit/36aeb3f93229488b0dbe3ea664d122c38031f835))
* supports git repo with `reftable` format ([79f6eed](https://github.com/albrtbc/gitstation/commit/79f6eed1caa03b9eeb07cabe955cafc5239c2b18))
* supports pre-fill short description for custom conventional commit types ([#2024](https://github.com/albrtbc/gitstation/issues/2024)) ([41bbefb](https://github.com/albrtbc/gitstation/commit/41bbefb8b7881355f72bf9f69013d3d2684ea1a9))
* supports to copy subjects of selected multi-commits ([2cbd63a](https://github.com/albrtbc/gitstation/commit/2cbd63a4a220ebb9c75c0c396a998b0c45d65363))
* supports to customize diff/merge options for external diff/merge tool ([#1971](https://github.com/albrtbc/gitstation/issues/1971)) ([4f8bdb4](https://github.com/albrtbc/gitstation/commit/4f8bdb4f55952451aa150177c9f7a70848e4ccec))
* supports to exclude some editors ([#2033](https://github.com/albrtbc/gitstation/issues/2033)) ([5a67a9f](https://github.com/albrtbc/gitstation/commit/5a67a9f1aaf9f34c13c9c0fc28b9c845c44504e9))
* supports to hide `AUTHOR/COMMIT TIME` column in `HISTORY` page ([a8da481](https://github.com/albrtbc/gitstation/commit/a8da481f694f350b9587ca9c558e3dc110fc52aa))
* supports to hide `AUTHOR` and `SHA` columns in `HISTORY` ([#2097](https://github.com/albrtbc/gitstation/issues/2097)) ([bf8e392](https://github.com/albrtbc/gitstation/commit/bf8e392bb2a5c8d1807e906ff2ad8cc26b599f22))
* supports to select two branches/tags to compare them ([94eb5db](https://github.com/albrtbc/gitstation/commit/94eb5db36f2b0f003d483110aafe7b04558d0cdd))
* supports to switch 24-hours/12-hours ([#2183](https://github.com/albrtbc/gitstation/issues/2183)) ([21d52c2](https://github.com/albrtbc/gitstation/commit/21d52c2b6b1447f4d0fd123f9bdd7991136fa1e5))
* use `<SOURCEGIT> --blame <FILE>` to launch `SourceGit` as `Blame` viewer ([#2047](https://github.com/albrtbc/gitstation/issues/2047)) ([7db1442](https://github.com/albrtbc/gitstation/commit/7db14420900fc66f8f1e6f4cadd35f3ca599715f))
* use `Ctrl + -/=` to zoom in/out window content ([#1810](https://github.com/albrtbc/gitstation/issues/1810)) ([bd79999](https://github.com/albrtbc/gitstation/commit/bd79999615a6fa1e429db784fab2b9b4982c13a3))
* webhook to release to homebrew ([#2010](https://github.com/albrtbc/gitstation/issues/2010)) ([120a331](https://github.com/albrtbc/gitstation/commit/120a331151e937a72bf249c33c4d7ddc56a4f8b6))


### Bug Fixes

* `DirectoryInfo.GetFiles` may raise `System.Reflection.TargetInvocationException` ([#2117](https://github.com/albrtbc/gitstation/issues/2117)) ([bf3503b](https://github.com/albrtbc/gitstation/commit/bf3503b1c4256cc73ad9f32120243fe2cccc7021))
* `Open in External Merge Tool` does not work for unstage change ([#2117](https://github.com/albrtbc/gitstation/issues/2117)) ([46d9a20](https://github.com/albrtbc/gitstation/commit/46d9a20fa678b949014ac673984f84aa1cc43682))
* add renamed resource files and fix .gitignore exclusions ([9df6fe4](https://github.com/albrtbc/gitstation/commit/9df6fe4033a70ad76368ea0981850af5799ecd34))
* always try to navigate to selected commit even if it is not changed in `Blame` ([#2017](https://github.com/albrtbc/gitstation/issues/2017)) ([1a4fae7](https://github.com/albrtbc/gitstation/commit/1a4fae7ae5ec5439170ee001e90c75e19d576b56))
* always use lower-cased key to get git config ([#2102](https://github.com/albrtbc/gitstation/issues/2102)) ([8d16897](https://github.com/albrtbc/gitstation/commit/8d1689760598aa155b82c98517caaad8b383795e))
* auto-navigating to first change sometimes does not work ([#1952](https://github.com/albrtbc/gitstation/issues/1952)) ([f318eca](https://github.com/albrtbc/gitstation/commit/f318ecaaccbdec9e65fb7ef38caefc581c43b594))
* avoid crashing when try to close a repository that has been deleted on disk ([311244b](https://github.com/albrtbc/gitstation/commit/311244b9ad52748681dbb53269ca8f03cb1af47a))
* can not type in command palette ([e71c5f2](https://github.com/albrtbc/gitstation/commit/e71c5f27518293e16a4c3f2db0b540c88610605b))
* changes list sometimes did not update ([#2150](https://github.com/albrtbc/gitstation/issues/2150)) ([a76a898](https://github.com/albrtbc/gitstation/commit/a76a898f881fc134a5125a0634f4b40d69d1d75d))
* checking if current worktree file changed ([283f84f](https://github.com/albrtbc/gitstation/commit/283f84f6e61877248f1b1613ef23d0357bab0336))
* closing alert shows in wrong condition ([738357b](https://github.com/albrtbc/gitstation/commit/738357b13b34f3c7ad1537c621d6057e826d874c))
* column headers mis-aligned with rows ([e5788d0](https://github.com/albrtbc/gitstation/commit/e5788d0957c2c6e1e40aef14040780fb6474ca43))
* Correct reset to parent revision functionality ([#2125](https://github.com/albrtbc/gitstation/issues/2125)) ([9698b66](https://github.com/albrtbc/gitstation/commit/9698b6605434919d71ba837fd57a008bfc7dd52b))
* crash when the pre-push hook is a broken symlink ([#1991](https://github.com/albrtbc/gitstation/issues/1991)) ([5f2984f](https://github.com/albrtbc/gitstation/commit/5f2984f983defb6f240e40e48e3251750ba26469))
* crash while reading JetBrains Toolbox's `state.json` ([#1999](https://github.com/albrtbc/gitstation/issues/1999)) ([eef1dcc](https://github.com/albrtbc/gitstation/commit/eef1dcccbf7dd49473cbc39668d8a3890845e62b))
* designer stops working ([ca39f61](https://github.com/albrtbc/gitstation/commit/ca39f6185d8e87204e80049b86d7f21dced28667))
* do not append tail if it is `null` ([6760119](https://github.com/albrtbc/gitstation/commit/6760119e6a6bdf948d9b7754cc8d6928f34a8d09))
* do not draw separator line if all subject has been scrolled out of display viewbox ([64d44ef](https://github.com/albrtbc/gitstation/commit/64d44efaf9f8f946e327a52bfff097d8f679cc80))
* enter infinite-loop when selecting the longest line in text editor with syntax-highlighting enabled ([#2161](https://github.com/albrtbc/gitstation/issues/2161)) ([dcca665](https://github.com/albrtbc/gitstation/commit/dcca665b2ee95bb7a0cdf6f3d8fa076e8acab580))
* error when trying to unstage files in fresh repo ([#2177](https://github.com/albrtbc/gitstation/issues/2177)) ([fd62fbe](https://github.com/albrtbc/gitstation/commit/fd62fbee7546f39e717dc722ba2b751f25a08460))
* extra empty lines in diff (Windows) ([#2015](https://github.com/albrtbc/gitstation/issues/2015)) ([df3acff](https://github.com/albrtbc/gitstation/commit/df3acffc06065035ec769fa544628a6993df2a46))
* failed to discard changes in a new file which has been staged partically ([#2189](https://github.com/albrtbc/gitstation/issues/2189)) ([1ca86ad](https://github.com/albrtbc/gitstation/commit/1ca86ad4b8f45229d64aebe242511844adb7c1c1))
* formatting issues flagged by dotnet format ([c7e1574](https://github.com/albrtbc/gitstation/commit/c7e15743d3e64f9ea899bc835bb0a93d4f2628a2))
* gets doubled EOLs while coping content with `CRLF` line-endings in diff view ([#2091](https://github.com/albrtbc/gitstation/issues/2091)) ([8a5b024](https://github.com/albrtbc/gitstation/commit/8a5b0245b6fd85b0a1f2459c69a6834301ddbd15))
* handling of last line without newline in hunks ([#1907](https://github.com/albrtbc/gitstation/issues/1907)) ([f88c424](https://github.com/albrtbc/gitstation/commit/f88c424c10e74707570062b90c602509ee19b2f8))
* highting selected revision lines in `Blame` window does not work ([e3c5789](https://github.com/albrtbc/gitstation/commit/e3c57895c790d386f10f930c8876700a9c4e86b1))
* last line change may disable hunk operation in `side-by-side` diff ([#2027](https://github.com/albrtbc/gitstation/issues/2027)) ([8dbe902](https://github.com/albrtbc/gitstation/commit/8dbe902b55b78035d66efd61de23b760332f57d8))
* launche's title does not update when last active page is the first one ([9ac6afa](https://github.com/albrtbc/gitstation/commit/9ac6afac0510d3106bc9e826ab5e3bdb4b9df232))
* left `LineNumberMargin` in text editor did not update its width ([3b576b0](https://github.com/albrtbc/gitstation/commit/3b576b02f548e792f4e22ae784e3113c692c324e))
* name of .editorconfig in solution ([#1961](https://github.com/albrtbc/gitstation/issues/1961)) ([286b8d4](https://github.com/albrtbc/gitstation/commit/286b8d4a95d5c045026145386decc8f69cdfff1d))
* parsing copied change ([#2174](https://github.com/albrtbc/gitstation/issues/2174)) ([baf85af](https://github.com/albrtbc/gitstation/commit/baf85af7c0a1d2b3d9a99a56f3f5db95f7cc99aa))
* pre-filled action does not update some attributes of `InteractiveRebaseItem` ([2ed83c5](https://github.com/albrtbc/gitstation/commit/2ed83c5e2cd64884d191a56d27f8c8023234c092))
* prevent `MeasureOverride` from returning `Infinity` to Avalonia layout ([#2136](https://github.com/albrtbc/gitstation/issues/2136)) ([#2137](https://github.com/albrtbc/gitstation/issues/2137)) ([2179a25](https://github.com/albrtbc/gitstation/commit/2179a25ab8d03cfd77fed6150ef215bda047e742))
* prevent cancel exception when quitting SourceGit ([#1962](https://github.com/albrtbc/gitstation/issues/1962)) ([ceb56d5](https://github.com/albrtbc/gitstation/commit/ceb56d5d178b87a92fa3bb2a5469b9a6e85ec641))
* recently introduced bug affecting `Block Navigation` ([#1948](https://github.com/albrtbc/gitstation/issues/1948)) ([ccfd7f0](https://github.com/albrtbc/gitstation/commit/ccfd7f0d3fa969a8d4e0f9e55eea701fdf61adff))
* rename resource files sourcegit.* → gitstation.* to match scripts ([9de6673](https://github.com/albrtbc/gitstation/commit/9de6673314a66a85aa34ffc5ead7138ee8f817fc))
* reset `desktop.ShutdownMode` to `ShutdownMode.OnExplicitShutdown` before calling `desktop.Shutdown` manually ([#2129](https://github.com/albrtbc/gitstation/issues/2129)) ([5f1825d](https://github.com/albrtbc/gitstation/commit/5f1825d4c386a2f9bfb5d57d64691255a8cb5c2e))
* set macOS (Intel) runner to `macos-15-intel` ([#1960](https://github.com/albrtbc/gitstation/issues/1960)) ([9fa6a7f](https://github.com/albrtbc/gitstation/commit/9fa6a7f9d6442308c10842da32862485c8ee78dd))
* sometimes line number margin's width does not update ([2c1b798](https://github.com/albrtbc/gitstation/commit/2c1b798f84c9150c5969a5e6e247f821f1a6b270))
* sometimes no staged/unstaged changes were shown ([#2030](https://github.com/albrtbc/gitstation/issues/2030)) ([42db2b1](https://github.com/albrtbc/gitstation/commit/42db2b16f5d5e37f13f3d5a98db30b993377e54b))
* support to switch blaming revision with renamed files ([#2040](https://github.com/albrtbc/gitstation/issues/2040)) ([96e2754](https://github.com/albrtbc/gitstation/commit/96e2754bac693f4690ccf9265abd5ed30269ca49))
* the `MinWidth` of author column changed after dragging the right border of it ([#1591](https://github.com/albrtbc/gitstation/issues/1591)) ([0409849](https://github.com/albrtbc/gitstation/commit/0409849a2553faa1045fcfe5deba581d65e00dc4))
* use `--ignore-space-change` instead of `--ignore-all-space` when `Ignore All Whitespace Changes` option is enabled ([#1752](https://github.com/albrtbc/gitstation/issues/1752)) ([4fcb12b](https://github.com/albrtbc/gitstation/commit/4fcb12bbf7cff19653dc8748bb74a737e0ac7725))
* worktree file status not updated ([#2011](https://github.com/albrtbc/gitstation/issues/2011)) ([a273cad](https://github.com/albrtbc/gitstation/commit/a273cad23bb9f7a811fa26477e782a55dc36b0b7))
* wrong alpha while reading `.dds` and `.tga` ([fbb9f78](https://github.com/albrtbc/gitstation/commit/fbb9f785397995b8f04be18c7bb7a9f493e76d40))
* wrong diff arguments for xcode `FileMerge` ([#1979](https://github.com/albrtbc/gitstation/issues/1979)) ([76e3df6](https://github.com/albrtbc/gitstation/commit/76e3df6d8dd39cf98e77b2fa5b27e1102b8644dc))


### Reverts

* "refactor: commit message editor ([#2096](https://github.com/albrtbc/gitstation/issues/2096))" ([df09ce0](https://github.com/albrtbc/gitstation/commit/df09ce01fadbee4e568174aa79aadb30d0c46fc5))


### Code Refactoring

* change `Show Tags as Tree` and `Show Submodules as Tree` setting from global to per-repo's UI state ([3f9b611](https://github.com/albrtbc/gitstation/commit/3f9b6114394ad9a34229342435cc762afc93f125))
* split repository settings into two parts - shared settings and ui states ([#2071](https://github.com/albrtbc/gitstation/issues/2071)) ([62d2908](https://github.com/albrtbc/gitstation/commit/62d2908a7908b49676fcf53c7ce6d3402d6ef05c))
